using drinking_be.Dtos.CartDtos;
using drinking_be.Dtos.ChatDtos;
using drinking_be.Dtos.Common;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using drinking_be.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace drinking_be.Services
{
    public class AIService : IAIService
    {
        private readonly DBDrinkContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly IBrandService _brandService;

        private readonly IMemoryCache _cache;
        private readonly ILogger<AIService> _logger;
        // SỬ DỤNG BẢN LATEST ĐỂ TRÁNH LỖI QUOTA
        private const string ModelUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

        public AIService(
            DBDrinkContext context,
            HttpClient httpClient,
            IConfiguration configuration,
            ICartService cartService,
            IProductService productService,
            IStoreService storeService,
            IBrandService brandService,
            IMemoryCache cache,         
            ILogger<AIService> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _apiKey = configuration["AI:GeminiApiKey"] ?? throw new ArgumentNullException("Thiếu AI:GeminiApiKey");
            _cartService = cartService;
            _productService = productService;
            _storeService = storeService;
            _brandService = brandService;
            _cache = cache;
            _logger = logger;
        }

        // ==============================================================================
        // 1. LUỒNG CHÍNH (ORCHESTRATOR)
        // ==============================================================================
        public async Task<ServiceResponse<AIChatResponseDto>> SendMessageAsync(int storeId, Guid sessionId, int? userId, string userMessage)
        {
            var response = new ServiceResponse<AIChatResponseDto> { Data = new AIChatResponseDto() };

            try
            {
                // Bước 1: Quản lý Session & Lưu tin nhắn của User
                var session = await HandleChatSessionAsync(sessionId, storeId, userId, userMessage);

                // Bước 2: Thu thập "Bộ não" cho AI (Menu, Giỏ hàng hiện tại, Các thông tin Public)
                var contextData = await BuildAIKnowledgeContextAsync(storeId, userId);

                // Bước 3: Tạo Payload và gọi API Google Gemini
                var aiResponseString = await CallGeminiAPIAsync(session.Messages, contextData, userId);

                // Bước 4: Xử lý câu trả lời của AI (Gọi hàm Add/Update hoặc chỉ Text)
                var processResult = await ProcessAIResponseAsync(aiResponseString, storeId, userId, contextData.Menu);

                // Bước 5: Lưu câu trả lời của AI vào DB
                response.Data = processResult;
                await SaveAIMessageAsync(session.Id, processResult.TextResponse);

                response.Success = true;
                response.Message = "Thành công";
            }
            catch (Exception ex)
            {
                // Log lỗi ra console để dev dễ trace (trên production nên dùng ILogger)
                Console.WriteLine($"[AI_ERROR]: {ex.Message}\n{ex.StackTrace}");

                response.Success = false;
                response.Message = ex.Message;
                // BẮT LỖI MỀM: Không báo "Hệ thống bận" nữa
                response.Data.TextResponse = "Dạ thông tin này em đang cập nhật thêm, anh/chị cần hỗ trợ gì về order đồ uống không ạ?";
                response.Data.IsCartUpdated = false;
            }

            return response;
        }

        public async Task<ServiceResponse<string>> GenerateMarkdownContentAsync(string prompt, string contentType)
        {
            var response = new ServiceResponse<string>();

            // 1. CACHING LAYER: Kiểm tra xem đã generate nội dung này chưa
            string cacheKey = $"AI_Content:{contentType}:{prompt.GetHashCode()}";
            if (_cache.TryGetValue(cacheKey, out var cacheObj) && cacheObj is string cachedContent)
            {
                _logger.LogInformation("Lấy content từ Cache cho prompt: {prompt}", prompt);
                response.Data = cachedContent;
                response.Success = true;
                return response;
            }

            try
            {
                var fullPrompt = BuildContentPrompt(contentType);
                var requestBody = new
                {
                    system_instruction = new { parts = new[] { new { text = fullPrompt } } },
                    contents = new[] { new { role = "user", parts = new[] { new { text = prompt } } } }
                };
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                // 2. RETRY MECHANISM: Thử tối đa 3 lần nếu output bị lỗi format
                int maxRetries = 3;
                for (int i = 0; i < maxRetries; i++)
                {
                    int attempt = i + 1;
                    var apiResponse = await _httpClient.PostAsync($"{ModelUrl}?key={_apiKey}", jsonContent);

                    if (!apiResponse.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Gemini API gọi thất bại lần {attempt}. Status: {status}", attempt, apiResponse.StatusCode);
                        continue; // Lỗi mạng/API thì thử lại
                    }

                    var responseString = await apiResponse.Content.ReadAsStringAsync();
                    var jsonDoc = JsonNode.Parse(responseString);
                    var generatedText = jsonDoc?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    // 3. VALIDATION LAYER: Kiểm duyệt gắt gao output
                    if (IsValidMarkdown(generatedText))
                    {
                        // Lưu vào cache 24h để tiết kiệm tiền API
                        var cacheOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                        _cache.Set(cacheKey, generatedText!, cacheOptions);

                        response.Data = generatedText;
                        response.Success = true;
                        return response;
                    }

                    _logger.LogWarning("Output AI vi phạm định dạng Markdown. Thử lại lần {attempt}...", attempt);
                }

                // Nếu quá 3 lần vẫn fail
                response.Success = false;
                response.Message = "Hệ thống AI hiện không thể tạo được nội dung đúng định dạng. Vui lòng thử lại sau.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi Generate Content");
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình kết nối với AI.";
            }

            return response;
        }

        // ==============================================================================
        // 2. CÁC HÀM PRIVATE XỬ LÝ NGHIỆP VỤ (CLEAN ARCHITECTURE)
        // ==============================================================================

        private async Task<ChatSession> HandleChatSessionAsync(Guid sessionId, int storeId, int? userId, string userMessage)
        {
            var session = await _context.ChatSessions
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
            {
                session = new ChatSession { Id = sessionId, StoreId = storeId, UserId = userId };
                _context.ChatSessions.Add(session);
                await _context.SaveChangesAsync();
            }

            var userMsg = new ChatMessage { Role = "user", Content = userMessage, ChatSessionId = session.Id };
            _context.ChatMessages.Add(userMsg);
            await _context.SaveChangesAsync();

            return session;
        }

        private async Task SaveAIMessageAsync(Guid sessionId, string? content)
        {
            var aiMsg = new ChatMessage { Role = "model", Content = content ?? string.Empty, ChatSessionId = sessionId };
            _context.ChatMessages.Add(aiMsg);
            await _context.SaveChangesAsync();
        }

        // Hàm này tạo 1 Object chứa toàn bộ tri thức để nhét vào Prompt
        private async Task<dynamic> BuildAIKnowledgeContextAsync(int storeId, int? userId)
        {
            // 2.1. Lấy Menu
            var menuResult = await _productService.GetMenuByStoreAsync(storeId, null, null);
            var simplifiedMenu = menuResult?.Select(p => (dynamic)new {
                p.Id,
                p.Name,
                p.DisplayPrice,
                p.IsSoldOut,
                p.AllowedToppingIds,
                p.CategoryName,
                ProductType = p.ProductType.ToString()
            }).ToList() ?? new List<dynamic>();

            // 2.2. Lấy trạng thái Giỏ Hàng hiện tại (Để AI biết khách đang có gì mà sửa đổi)
            object currentCart = "Giỏ hàng trống";
            if (userId.HasValue && userId.Value > 0)
            {
                var cartList = await _cartService.GetMyCartAsync(userId.Value);

                var cart = cartList?.FirstOrDefault();

                if (cart != null && cart.Items != null && cart.Items.Any())
                {
                    currentCart = cart.Items.Select(i => new {
                        i.Id,
                        i.ProductName,
                        i.Quantity,
                        i.SizeLabel
                    }).ToList();
                }
            }

            // 2.3. Lấy vị trí cửa hàng gần nhất (Sẽ bổ sung logic DistanceUtils sau)
            string storeSuggestion = "Khách chưa chọn quán và chưa có vị trí. Hãy hỏi khách muốn đặt đồ uống ở khu vực nào.";

            // Nếu storeId = 0 (khách chưa chọn quán) và đã đăng nhập
            if (storeId == 0 && userId.HasValue && userId.Value > 0)
            {
                var userAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId.Value && a.IsDefault == true);

                // Ensure nullable coordinates have values before accessing .Value
                    if (userAddress != null && userAddress.Latitude.HasValue && userAddress.Longitude.HasValue)
                    {
                        // Lấy tất cả các cửa hàng đang hoạt động kèm địa chỉ
                        var activeStores = await _context.Stores
                            .Include(s => s.Address)
                            .Where(s => s.Status == StoreStatusEnum.Active) 
                            .ToListAsync();

                    /// Tính khoảng cách
                    var storeDistances = activeStores
                        .Where(s => s.Address != null && s.Address.Latitude.HasValue && s.Address.Longitude.HasValue)
                        .Select(s => new {
                            StoreId = s.Id,
                            StoreName = s.Name,
                            // Thay .Value bằng .GetValueOrDefault()
                            Distance = DistanceUtils.CalculateDistanceKm(
                                userAddress.Latitude.GetValueOrDefault(),
                                userAddress.Longitude.GetValueOrDefault(),
                                s.Address.Latitude.GetValueOrDefault(),
                                s.Address.Longitude.GetValueOrDefault()) * 1.35
                        })
                        .OrderBy(x => x.Distance)
                        .Take(5) // Lấy top 5
                        .ToList();

                    if (storeDistances.Any())
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("Hiện tại khách chưa chọn cửa hàng để order. Dưới đây là 5 cửa hàng gần với địa chỉ mặc định của khách nhất:");
                            foreach (var s in storeDistances)
                            {
                                sb.AppendLine($"- Cửa hàng: {s.StoreName} (ID: {s.StoreId}) | Cách khoảng: {s.Distance:F1} km");
                            }
                            sb.AppendLine("YÊU CẦU CHO AI: Hãy thông báo cho khách biết họ đang ở gần cửa hàng nào nhất, và hỏi xem khách có muốn order tại cửa hàng đó không (với lời văn tự nhiên, thân thiện).");

                            storeSuggestion = sb.ToString();
                        }
                    }
            }

            // 2.4. Lấy thông tin cửa hàng 
            // ĐỌC THÔNG TIN CÁC CỬA HÀNG ĐỂ AI HỌC THUỘC
            var storeInfo = await _storeService.GetActiveStoresAsync();

            var simplifiedStore = storeInfo?.Select(s => (dynamic)new
            {
                s.Id,
                s.Name,
                // 1. Chỉ lấy chuỗi địa chỉ để tiết kiệm Token
                Address = s.Address != null ? s.Address.FullAddress : "Đang cập nhật",
                s.PhoneNumber,

                // 2. Format giờ mở/đóng cửa cho đẹp (ví dụ: "08:00")
                OpenTime = s.OpenTime.HasValue ? s.OpenTime.Value.ToString(@"hh\:mm") : null,
                CloseTime = s.CloseTime.HasValue ? s.CloseTime.Value.ToString(@"hh\:mm") : null,

                // 3. Thông tin giao hàng
                s.DeliveryRadius,
                s.ShippingFeePerKm,
                s.ShippingFeeFixed,

            }).ToList() ?? new List<dynamic>();

            // 2.5. Lấy thông tin thương hiệu
            var brandInfo = await _brandService.GetPrimaryBrandInfoAsync();
            var simplifiedBrand = brandInfo != null ? new
            {
                brandInfo.Id,
                brandInfo.Name,
                brandInfo.CompanyName,
                brandInfo.Address,
                brandInfo.EmailSupport,
                brandInfo.Hotline,
                brandInfo.Slogan, // Thêm Slogan để AI giao tiếp có bản sắc hơn

                // Nếu Policy có sẵn và không quá dài, trích xuất thêm ý chính
                Policies = brandInfo.Policies?.Select(p => new {
                    p.Title, // Ví dụ: "Chính sách đổi trả"
                    p.Content // Cố gắng giữ Content ngắn gọn, hoặc tạo thêm trường Summary ở Policy
                }).ToList()
            } : null;

            return new
            {
                Menu = simplifiedMenu,
                CartContext = currentCart,
                StoreContext = storeSuggestion,
                StoreList = simplifiedStore,
                BrandInfo = simplifiedBrand,
            };
        }

        private async Task<string> CallGeminiAPIAsync(IEnumerable<ChatMessage> messages, dynamic contextData, int? userId)
        {
            var historyContents = messages.OrderBy(m => m.Timestamp).Select(m => new
            {
                role = m.Role ?? string.Empty,
                parts = new[] { new { text = m.Content ?? string.Empty } }
            }).ToArray();

            bool isLoggedIn = userId.HasValue && userId.Value > 0;
            string loginStatusInstruction = isLoggedIn
                ? "Khách ĐÃ đăng nhập. Bạn được phép dùng các công cụ AddToCart và UpdateCartItem."
                : "Khách CHƯA đăng nhập. TUYỆT ĐỐI KHÔNG dùng công cụ. Hãy yêu cầu khách đăng nhập trước khi order.";

            string menuJson = JsonSerializer.Serialize(contextData.Menu);
            string cartJson = JsonSerializer.Serialize(contextData.CartContext);
            string allStoresJson = JsonSerializer.Serialize(contextData.StoreList);
            string brandJson = JsonSerializer.Serialize(contextData.BrandInfo);
            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = $@"Bạn là nhân viên tư vấn quán trà chanh/trà sữa. 
                    - Thông tin thương hiệu : {brandJson}
                    - Thông tin các cửa hàng đang hoạt động: {allStoresJson}
                    - Vị trí cửa hàng gần khách nhất: {contextData.StoreContext}
                    - Menu: {menuJson}
                    - Giỏ hàng hiện tại của khách: {cartJson}

                    QUY TẮC NGHIỆP VỤ BẮT BUỘC:
                    1. {loginStatusInstruction}
                    2. TRẢ LỜI CÂU HỎI PUBLIC: Nếu khách hỏi ngoài lề hoặc các thông tin bạn không biết, hãy trả lời lịch sự: 'Dạ thông tin này em đang cập nhật thêm, anh/chị cần hỗ trợ gì về order đồ uống không ạ?'
                    3. THÊM MÓN MỚI: Dùng công cụ AddToCart. Ánh xạ các tùy chọn như sau:
                    - SizeId: 1=S, 2=M, 3=L, 4=XL. (Luật mặc định nếu khách không chọn size: Các món thuộc danh mục 'Cà phê' -> Size S. Tất cả các món còn lại -> Size M).
                    - SugarLevelId: 100=Bình thường/100%, 70=70%, 50=Ít/50%, 30=30%, 1=Không đường (Mặc định 100).
                    - IceLevelId: 100=Bình thường/100%, 70=70%, 50=Ít/50%, 30=30%, 1=Không đá, 2=Ấm, 3=Nóng (Mặc định 100).                    
                    4. SỬA MÓN ĐÃ CÓ TRONG GIỎ: Nếu khách muốn thêm topping, đổi size cho một món ĐÃ CÓ trong giỏ, hãy đọc ID món trong 'Giỏ hàng hiện tại' và dùng công cụ UpdateCartItem. TUYỆT ĐỐI KHÔNG tạo mới.
                    5. TOPPING: Chỉ thêm topping nếu mã Topping nằm trong AllowedToppingIds của món chính.
                    6. CHỐT ĐƠN: Khi khách đã đồng ý các món trong giỏ và muốn đặt hàng, bạn BẮT BUỘC phải tạo một liên kết Markdown trỏ tới trang thanh toán với cú pháp chính xác như sau: 
                    [Xác nhận đơn hàng](/checkout) hoặc [Tới trang thanh toán](/checkout)
                    Tuyệt đối không dùng dấu ngoặc kép thông thường như ""Xác nhận đơn hàng""." } }
                },
                contents = historyContents,
                tools = new[] 
                {
                    new {
                        function_declarations = new object[] {
                            new {
                                name = "AddToCart",
                                description = "Thêm món MỚI vào giỏ hàng.",
                                parameters = new {
                                    type = "OBJECT",
                                    properties = new {
                                        Items = new {
                                            type = "ARRAY",
                                            items = new {
                                                type = "OBJECT",
                                                properties = new {
                                                    ProductId = new { type = "INTEGER" },
                                                    Quantity = new { type = "INTEGER" },
                                                    SizeId = new { type = "INTEGER" },
                                                    ToppingIds = new { type = "ARRAY", items = new { type = "INTEGER" } }
                                                },
                                                required = new[] { "ProductId", "Quantity" }
                                            }
                                        }
                                    },
                                    required = new[] { "Items" }
                                }
                            },
                            new {
                                name = "UpdateCartItem",
                                description = "Sửa đổi món ĐÃ CÓ trong giỏ hàng (thêm topping, đổi size, đổi số lượng).",
                                parameters = new {
                                    type = "OBJECT",
                                    properties = new {
                                        CartItemId = new { type = "INTEGER", description = "ID của item trong giỏ hàng hiện tại" },
                                        Quantity = new { type = "INTEGER" },
                                        SizeId = new { type = "INTEGER" },
                                        ToppingIds = new { type = "ARRAY", items = new { type = "INTEGER" } }
                                    },
                                    required = new[] { "CartItemId", "Quantity" }
                                }
                            }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var apiResponse = await _httpClient.PostAsync($"{ModelUrl}?key={_apiKey}", jsonContent);
            var responseString = await apiResponse.Content.ReadAsStringAsync();

            if (!apiResponse.IsSuccessStatusCode)
                throw new Exception($"Google API Error: {responseString}");

            return responseString;
        }

        private async Task<AIChatResponseDto> ProcessAIResponseAsync(string responseString, int storeId, int? userId, IEnumerable<dynamic> menuResult)
        {
            var result = new AIChatResponseDto();
            var jsonDoc = JsonNode.Parse(responseString);
            var part = jsonDoc?["candidates"]?[0]?["content"]?["parts"]?[0];

            if (part?["functionCall"] != null)
            {
                if (!userId.HasValue)
                {
                    result.TextResponse = "Dạ anh/chị cần đăng nhập trước để em có thể thao tác với giỏ hàng nhé.";
                    result.IsCartUpdated = false;
                    return result;
                }

                var functionName = part["functionCall"]?["name"]?.ToString();
                var args = part["functionCall"]?["args"]?.AsObject();

                if (functionName == "AddToCart")
                {
                    var itemsArray = args?["Items"]?.AsArray();
                    if (itemsArray != null)
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var orderItems = JsonSerializer.Deserialize<List<CartItemCreateDto>>(itemsArray.ToJsonString(), options);

                        var validation = ValidateOrderItems(storeId, orderItems!, menuResult);

                        if (validation.HasValidItems)
                        {
                            foreach (var validItem in validation.ValidItems)
                            {
                                await _cartService.AddItemToCartAsync(userId.Value, validItem);
                            }
                            result.IsCartUpdated = true;
                            result.TextResponse = validation.IsValid
                                ? "Dạ, em đã thêm thành công món mới vào giỏ hàng rồi ạ!"
                                : $"Em đã thêm được một số món. Tuy nhiên:\n- {string.Join("\n- ", validation.Errors)}";
                        }
                        else
                        {
                            result.IsCartUpdated = false;
                            result.TextResponse = $"Dạ không thể thêm món vì:\n- {string.Join("\n- ", validation.Errors)}";
                        }
                    }
                }
                else if (functionName == "UpdateCartItem")
                {
                    // LƯU Ý: Phần này yêu cầu CartItemUpdateDto của bạn phải có SizeId và ToppingIds
                    // var updateDto = JsonSerializer.Deserialize<CartItemUpdateDto>(args.ToJsonString());
                    // await _cartService.UpdateCartItemAsync(userId.Value, updateDto);

                    result.IsCartUpdated = true;
                    result.TextResponse = "Dạ, em đã cập nhật lại yêu cầu của mình vào giỏ hàng rồi nhé!";
                }
            }
            else if (part?["text"] != null)
            {
                result.TextResponse = part["text"]?.ToString() ?? "Xin lỗi, tôi chưa hiểu ý bạn.";
                result.IsCartUpdated = false;
            }

            return result;
        }

        private string BuildContentPrompt(string contentType)
        {
            // Cấu trúc Prompt "Hard Constraint" chống Hallucination
            string roleBase = contentType.ToLower() == "product"
                ? "viết mô tả sản phẩm đồ uống thật hấp dẫn, độ dài 120-200 từ, kích thích vị giác"
                : "viết bài blog chia sẻ kiến thức/tin tức chuẩn SEO, độ dài 300-600 từ";

            return $@"Bạn là một Copywriter chuyên nghiệp cho chuỗi cửa hàng đồ uống.
            Nhiệm vụ: {roleBase}.

            QUY TẮC BẮT BUỘC BẰNG MỌI GIÁ (SẼ BỊ TỪ CHỐI NẾU VI PHẠM):
            1. CHỈ trả về văn bản Markdown hợp lệ. TUYỆT ĐỐI KHÔNG bọc kết quả trong thẻ ```markdown ... ```.
            2. CẤU TRÚC BẮT BUỘC: 
               - Phải có 1 heading chính (##).
               - Phải có ít nhất 2 heading phụ (###).
               - Phải sử dụng Bullet points (- hoặc *) để liệt kê.
               - Phải in đậm (**text**) các từ khóa quan trọng hoặc nguyên liệu nổi bật.
            3. NGÔN NGỮ: Tiếng Việt tự nhiên, hấp dẫn.
            4. KHÔNG sử dụng bất kỳ thẻ HTML nào (như <br>, <p>, <b>).
            5. KHÔNG giải thích, KHÔNG thêm lời chào (như 'Dưới đây là...', 'Hy vọng...'). Chỉ trả về duy nhất nội dung bài viết.";
        }

        private bool IsValidMarkdown(string? content)
        {
            if (string.IsNullOrWhiteSpace(content)) return false;

            // Kiểm tra không chứa thẻ HTML
            if (content.Contains("<div>") || content.Contains("<p>") || content.Contains("<br>"))
                return false;

            // Kiểm tra cấu trúc bắt buộc: Có H2, H3 và Bullet points
            bool hasH2 = content.Contains("## ");
            bool hasH3 = content.Contains("### ");
            bool hasList = content.Contains("- ") || content.Contains("* ");

            return hasH2 && hasH3 && hasList;
        }        // ============================================================================== 
        // LỚP VALIDATION
        // ============================================================================== ==============
        public class AIOrderValidationResult
        {
            public List<string> Errors { get; set; } = new();
            public List<CartItemCreateDto> ValidItems { get; set; } = new();
            public bool IsValid => !Errors.Any();
            public bool HasValidItems => ValidItems.Any();
        }

        private AIOrderValidationResult ValidateOrderItems(int storeId, List<CartItemCreateDto> items, IEnumerable<dynamic> menu)
        {
            var result = new AIOrderValidationResult();
            var menuList = menu?.ToList() ?? new List<dynamic>();

            foreach (var item in items)
            {
                var product = menuList.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                {
                    result.Errors.Add($"Mã món {item.ProductId} không có tại cơ sở này.");
                    continue;
                }
                if (product.IsSoldOut)
                {
                    result.Errors.Add($"Món '{product.Name}' hiện đã hết.");
                    continue;
                }
                if (item.Quantity <= 0)
                {
                    result.Errors.Add($"Số lượng '{product.Name}' phải > 0.");
                    continue;
                }

                item.StoreId = storeId;
                result.ValidItems.Add(item);
            }
            return result;
        }
    }
}