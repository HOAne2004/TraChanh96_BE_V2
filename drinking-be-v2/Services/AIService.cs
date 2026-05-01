using drinking_be.Dtos.CartDtos;
using drinking_be.Dtos.ChatDtos;
using drinking_be.Dtos.Common;
using drinking_be.Dtos.ProductDtos;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace drinking_be.Services
{
    public class AIService : IAIService
    {
        private readonly DBDrinkContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ICartService _cartService;

        // Bạn hãy đổi IProductService thành Interface lấy Menu thực tế của dự án nhé
        private readonly IProductService _productService;

        // SỬ DỤNG BẢN LATEST ĐỂ TRÁNH LỖI QUOTA
        private const string ModelUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

        public AIService(
            DBDrinkContext context,
            HttpClient httpClient,
            IConfiguration configuration,
            ICartService cartService,
            IProductService productService)
        {
            _context = context;
            _httpClient = httpClient;
            _apiKey = configuration["AI:GeminiApiKey"] ?? throw new ArgumentNullException("Thiếu AI:GeminiApiKey");
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<ServiceResponse<AIChatResponseDto>> SendMessageAsync(int storeId, Guid sessionId, int? userId, string userMessage)
        {
            var response = new ServiceResponse<AIChatResponseDto> { Data = new AIChatResponseDto() };

            try
            {
                // 1. LẤY HOẶC TẠO MỚI PHIÊN CHAT TỪ DATABASE
                var session = await _context.ChatSessions
                    .Include(s => s.Messages)
                    .FirstOrDefaultAsync(s => s.Id == sessionId);

                if (session == null)
                {
                    session = new ChatSession { Id = sessionId, StoreId = storeId, UserId = userId };
                    _context.ChatSessions.Add(session);
                    // Phải lưu Session mới trước để đảm bảo có bản ghi trong DB
                    await _context.SaveChangesAsync();
                }

                // CÁCH FIX LỖI EF CORE: Thêm Message bằng DbSet trực tiếp
                var userMsg = new ChatMessage { Role = "user", Content = userMessage, ChatSessionId = session.Id };
                _context.ChatMessages.Add(userMsg);
                await _context.SaveChangesAsync();

                // 2. CHUẨN BỊ NGỮ CẢNH (RAG - LẤY MENU CỬA HÀNG)
                var menuResult = await _productService.GetMenuByStoreAsync(storeId, null, null);
                string systemContext = "Hệ thống đang bảo trì Menu.";
                if (menuResult != null && menuResult.Any())
                {
                    var simplifiedMenu = menuResult.Select(p => new {
                        p.Id,
                        p.Name,
                        p.DisplayPrice,
                        p.IsSoldOut
                    }).ToList();
                    systemContext = JsonSerializer.Serialize(simplifiedMenu);
                }

                // 3. TẠO PAYLOAD GỬI LÊN GEMINI
                var historyContents = session.Messages.OrderBy(m => m.Timestamp).Select(m => new
                {
                    role = m.Role,
                    parts = new[] { new { text = m.Content } }
                }).ToArray();

                // LOGIC CHẶN ĐẶT HÀNG NẾU CHƯA ĐĂNG NHẬP
                bool isLoggedIn = userId.HasValue && userId.Value > 0;
                string loginStatusInstruction = isLoggedIn
                    ? "Khách đã đăng nhập. BẠN ĐƯỢC PHÉP dùng công cụ AddToCart."
                    : "Khách CHƯA đăng nhập. TUYỆT ĐỐI KHÔNG dùng AddToCart. Hãy lịch sự yêu cầu khách đăng nhập.";

                var requestBody = new
                {
                    system_instruction = new
                    {
                        parts = new[] { new { text = $@"Bạn là nhân viên chốt đơn chuyên nghiệp. Menu (JSON): {systemContext}. 

                            QUY TẮC BẮT BUỘC:
                            1. CHỈ sử dụng ProductId có trong JSON. TUYỆT ĐỐI KHÔNG tự bịa ProductId.
                            2. KIỂM TRA TỒN KHO: Nếu món IsSoldOut = true, TỪ CHỐI thêm vào giỏ, báo khách đã hết và gợi ý món khác.
                            3. YÊU CẦU CHUNG CHUNG: Nếu khách nói 'cho 1 ly trà', liệt kê 2-3 món để hỏi lại, KHÔNG tự ý chọn bừa.
                            4. {loginStatusInstruction}
                            5. Ánh xạ Size, Đường, Đá sang ID dạng số (short): SizeId: 1(S), 2(M), 3(L) | SugarLevelId/IceLevelId: 1(Bình thường), 2(Ít), 3(Không). Nếu khách không nói thì để null." } }
                    },
                    contents = historyContents,
                    tools = new[]
                            {
                new {
                    function_declarations = new[] {
                        new {
                            name = "AddToCart",
                            description = "Chỉ gọi hàm này khi khách muốn thêm món vào giỏ hàng VÀ khách ĐÃ đăng nhập.",
                            parameters = new {
                                type = "OBJECT",
                                properties = new {
                                    Items = new {
                                        type = "ARRAY",
                                        items = new {
                                            type = "OBJECT",
                                            properties = new {
                                                ProductId = new { type = "INTEGER", description = "ID của món" },
                                                Quantity = new { type = "INTEGER", description = "Số lượng" },
                                                SizeId = new { type = "INTEGER", description = "ID của Size" },
                                                SugarLevelId = new { type = "INTEGER", description = "ID mức đường" },
                                                IceLevelId = new { type = "INTEGER", description = "ID mức đá" },
                                                Note = new { type = "STRING", description = "Ghi chú thêm" }
                                            },
                                            required = new[] { "ProductId", "Quantity" }
                                        }
                                    }
                                },
                                required = new[] { "Items" }
                            }
                        }
                    }
                }
            }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                // 4. GỌI API GOOGLE
                var apiResponse = await _httpClient.PostAsync($"{ModelUrl}?key={_apiKey}", jsonContent);
                var responseString = await apiResponse.Content.ReadAsStringAsync();

                if (!apiResponse.IsSuccessStatusCode)
                    throw new Exception($"Gemini Lỗi: {responseString}");

                // 5. PHÂN TÍCH KẾT QUẢ VÀ THỰC THI
                var jsonDoc = JsonNode.Parse(responseString);
                var part = jsonDoc?["candidates"]?[0]?["content"]?["parts"]?[0];

                if (part?["functionCall"] != null)
                {
                    if (!isLoggedIn)
                    {
                        response.Data.TextResponse = "Dạ, anh/chị cần đăng nhập trước để em có thể thêm món vào giỏ hàng nhé.";
                        response.Data.IsCartUpdated = false;
                    }
                    else
                    {
                        var args = part["functionCall"]?["args"]?.AsObject();
                        var itemsArray = args?["Items"]?.AsArray();

                        if (itemsArray != null)
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var orderItems = JsonSerializer.Deserialize<List<CartItemCreateDto>>(itemsArray.ToJsonString(), options);

                            if (orderItems != null && orderItems.Any())
                            {
                                // 🔥 ĐƯA VÀO LƯỚI LỌC VALIDATION (Truyền menuResult vào để không phải query DB lại)
                                var validation = ValidateOrderItems(storeId, orderItems, menuResult ?? new List<StoreMenuReadDto>());

                                if (validation.HasValidItems)
                                {
                                    // Lưu các món hợp lệ vào DB
                                    foreach (var validItem in validation.ValidItems)
                                    {
                                        await _cartService.AddItemToCartAsync(userId.Value, validItem);
                                    }
                                    response.Data.IsCartUpdated = true;

                                    // Xử lý thông báo (Thành công toàn bộ vs Thành công 1 phần)
                                    if (validation.IsValid)
                                    {
                                        response.Data.TextResponse = "Dạ, em đã thêm đầy đủ các món vào giỏ hàng cho mình rồi ạ!";
                                    }
                                    else
                                    {
                                        response.Data.TextResponse = $"Em đã thêm được một số món vào giỏ hàng. Tuy nhiên có vài lỗi nhỏ:\n- {string.Join("\n- ", validation.Errors)}\nAnh/chị kiểm tra lại giỏ hàng nhé!";
                                    }
                                }
                                else
                                {
                                    // Lỗi toàn bộ (Ví dụ gọi 1 món và món đó hết hàng)
                                    response.Data.IsCartUpdated = false;
                                    response.Data.TextResponse = $"Dạ không thể thêm món vào giỏ hàng vì:\n- {string.Join("\n- ", validation.Errors)}";
                                }
                            }
                        }
                    }
                }
                else if (part?["text"] != null)
                {
                    response.Data.TextResponse = part["text"]?.ToString() ?? "Xin lỗi, tôi chưa hiểu ý bạn.";
                    response.Data.IsCartUpdated = false;
                }

                // 6. LƯU CÂU TRẢ LỜI CỦA AI VÀO DATABASE
                var aiMsg = new ChatMessage { Role = "model", Content = response.Data.TextResponse, ChatSessionId = session.Id };
                _context.ChatMessages.Add(aiMsg);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data.TextResponse = "Dạ hệ thống em đang bị gián đoạn một chút, bạn vui lòng thử lại sau nhé.";
            }

            return response;
        }

        public class AIOrderValidationResult
        {
            public List<string> Errors { get; set; } = new();
            public List<CartItemCreateDto> ValidItems { get; set; } = new();
            public bool IsValid => !Errors.Any();
            public bool HasValidItems => ValidItems.Any();
        }

        private AIOrderValidationResult ValidateOrderItems(
            int storeId,
            List<CartItemCreateDto> items,
            IEnumerable<StoreMenuReadDto> menu)
        {
            var result = new AIOrderValidationResult();

            foreach (var item in items)
            {
                // Kiểm tra món có tồn tại trong Menu của cửa hàng không
                var product = menu.FirstOrDefault(p => p.Id == item.ProductId);

                if (product == null)
                {
                    result.Errors.Add($"Mã món {item.ProductId} không tồn tại hoặc không bán tại cơ sở này.");
                    continue;
                }

                if (product.IsSoldOut)
                {
                    result.Errors.Add($"Món '{product.Name}' hiện đã hết nguyên liệu.");
                    continue;
                }

                // Validate Size (Bạn cấu hình 1,2,3 tương ứng S,M,L)
                if (item.SizeId.HasValue && (item.SizeId < 1 || item.SizeId > 4))
                {
                    result.Errors.Add($"Size chọn cho món '{product.Name}' không hợp lệ.");
                    continue;
                }

                if (item.Quantity <= 0)
                {
                    result.Errors.Add($"Số lượng món '{product.Name}' phải lớn hơn 0.");
                    continue;
                }

                item.StoreId = storeId;
                result.ValidItems.Add(item);
            }

            return result;
        }
    }
}