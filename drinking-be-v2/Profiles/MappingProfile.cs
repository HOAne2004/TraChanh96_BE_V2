using AutoMapper;
using drinking_be.Dtos.AddressDtos;
using drinking_be.Dtos.AttendanceDtos;
using drinking_be.Dtos.BannerDtos;
using drinking_be.Dtos.BrandDtos;
using drinking_be.Dtos.CartDtos;
using drinking_be.Dtos.CategoryDtos;
using drinking_be.Dtos.CommentDtos;
using drinking_be.Dtos.FranchiseDtos;
using drinking_be.Dtos.InventoryDtos;
using drinking_be.Dtos.MaterialDtos;
using drinking_be.Dtos.MembershipDtos;
using drinking_be.Dtos.MembershipLevelDtos;
using drinking_be.Dtos.NewsDtos;
using drinking_be.Dtos.NotificationDtos;
using drinking_be.Dtos.OrderDtos;
using drinking_be.Dtos.OrderItemDtos;
using drinking_be.Dtos.OrderPaymentDtos;
using drinking_be.Dtos.PaymentMethodDtos;
using drinking_be.Dtos.PayslipDtos;
using drinking_be.Dtos.PolicyDtos;
using drinking_be.Dtos.ProductDtos;
using drinking_be.Dtos.ReservationDtos;
using drinking_be.Dtos.ReviewDtos;
using drinking_be.Dtos.RoomDtos;
using drinking_be.Dtos.ShopTableDtos;
using drinking_be.Dtos.SizeDtos;
using drinking_be.Dtos.SocialMediaDtos;
using drinking_be.Dtos.StaffDtos;
using drinking_be.Dtos.StoreDtos;
using drinking_be.Dtos.SupplyOrderDtos;
using drinking_be.Dtos.UserDtos;
using drinking_be.Dtos.VoucherDtos;
using drinking_be.Enums;
using drinking_be.Models;
using drinking_be.Utils;
using System;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // --- Address Mappings ---
        CreateMap<AddressCreateDto, Address>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PublicStatusEnum.Active))
            .ReverseMap();
        CreateMap<Address, AddressReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<AddressUpdateDto, Address>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Attendance Mappings ---
        CreateMap<AttendanceCreateDto, Attendance>().ReverseMap();
        CreateMap<Attendance, AttendanceReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff.FullName))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name));
        CreateMap<AttendanceUpdateDto, Attendance>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Banner Mappings ---
        CreateMap<Banner, BannerReadDto>();
        CreateMap<BannerCreateDto, Banner>();

        // --- Brand Mappings ---
        CreateMap<BrandCreateDto, Brand>().ReverseMap();
        CreateMap<Brand, BrandReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<BrandUpdateDto, Brand>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Cart Mappings ---
        CreateMap<Cart, CartReadDto>()
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore());

        // --- CartItem Mappings ---
        CreateMap<CartItemCreateDto, CartItem>()
             .ForMember(dest => dest.CartId, opt => opt.Ignore())
             .ForMember(dest => dest.SugarLevel, opt => opt.MapFrom(src => src.SugarLevelId.HasValue ? (SugarLevelEnum)src.SugarLevelId.Value : SugarLevelEnum.S100))
             .ForMember(dest => dest.IceLevel, opt => opt.MapFrom(src => src.IceLevelId.HasValue ? (IceLevelEnum)src.IceLevelId.Value : IceLevelEnum.I100));
        CreateMap<CartItemUpdateDto, CartItem>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CartItem, CartItemReadDto>()
            .ForMember(dest => dest.ProductName, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.SizeLabel, opt => opt.Ignore())
            .ForMember(dest => dest.Toppings, opt => opt.Ignore())
            .ForMember(dest => dest.SugarLabel, opt => opt.MapFrom(src => src.SugarLevel.ToString()))
            .ForMember(dest => dest.IceLabel, opt => opt.MapFrom(src => src.IceLevel.ToString()));
        CreateMap<CartItem, CartToppingReadDto>()
            .ForMember(dest => dest.ProductName, opt => opt.Ignore());

        // --- Category Mappings ---
        CreateMap<CategoryCreateDto, Category>()
             .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Name.ToLower().Replace(" ", "-")))
             .ReverseMap();
        CreateMap<Category, CategoryReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.InverseParent));
        CreateMap<CategoryUpdateDto, Category>()
            .ForMember(dest => dest.Slug, opt => opt.Condition(src => src.Slug != null || src.Name != null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Comment Mappings ---
        CreateMap<CommentCreateDto, Comment>()
             .ForMember(dest => dest.UserId, opt => opt.Ignore())
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReviewStatusEnum.Pending))
             .ReverseMap();
        CreateMap<Comment, CommentReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserThumbnailUrl, opt => opt.MapFrom(src => src.User.ThumbnailUrl))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.InverseParent));

        // --- FranchiseRequest Mappings ---
        CreateMap<FranchiseCreateDto, FranchiseRequest>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => FranchiseStatusEnum.Pending))
            .ReverseMap();
        CreateMap<FranchiseRequest, FranchiseReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.Username : "Chưa phân công"));
        CreateMap<FranchiseUpdateDto, FranchiseRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Inventory Mappings ---
        CreateMap<InventoryCreateDto, Inventory>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ReverseMap();
        CreateMap<Inventory, InventoryReadDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material.Name))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Material.BaseUnit))
            .ForMember(dest => dest.MaterialImageUrl, opt => opt.MapFrom(src => src.Material.ImageUrl))
            .ForMember(dest => dest.MinStockAlert, opt => opt.MapFrom(src => src.Material.MinStockAlert))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : "Kho Tổng (HQ)"))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.Material.MinStockAlert.HasValue && src.Quantity <= src.Material.MinStockAlert.Value));
        CreateMap<InventoryUpdateDto, Inventory>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // --- Material Mappings ---
        CreateMap<MaterialCreateDto, Material>()
            .ForMember(dest => dest.PublicId, opt => opt.Ignore())
            .ForMember(dest => dest.Inventories, opt => opt.Ignore())
            .ForMember(dest => dest.SupplyOrderItems, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<Material, MaterialReadDto>()
            .ForMember(dest => dest.CostPerBaseUnit, opt => opt.MapFrom(src => src.CostPerBaseUnit));
        CreateMap<MaterialUpdateDto, Material>()
            .ForMember(dest => dest.PublicId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- MembershipLevel Mappings ---
        CreateMap<MembershipLevelCreateDto, MembershipLevel>().ReverseMap();
        CreateMap<MembershipLevel, MembershipLevelReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<MembershipLevelUpdateDto, MembershipLevel>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Membership Mappings ---
        CreateMap<MembershipCreateDto, Membership>()
            .ForMember(dest => dest.CardCode, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<Membership, MembershipReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.Level.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));

        // --- News Mappings ---
        CreateMap<NewsCreateDto, News>()
             .ForMember(dest => dest.Slug, opt => opt.Ignore())
             .ForMember(dest => dest.PublishedDate, opt => opt.Condition(src => src.Status == ContentStatusEnum.Published))
             .ReverseMap();
        CreateMap<News, NewsReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
        CreateMap<NewsUpdateDto, News>()
            .ForMember(dest => dest.PublishedDate, opt => opt.Condition((src, dest) => src.Status == ContentStatusEnum.Published && dest.PublishedDate == null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Notification Mappings ---
        CreateMap<Notification, NotificationReadDto>();
        CreateMap<NotificationCreateDto, Notification>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString())); // Enum -> String

        ;// --- Order Mappings ---
        CreateMap<Order, OrderReadDto>()
            // 1. Map các Enum (Giữ nguyên giá trị số)
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType))
            .ForMember(dest => dest.CancelReason, opt => opt.MapFrom(src => src.CancelReason))

            // 2. Map các Label (Dùng Extension Method để lấy tiếng Việt)
            .ForMember(dest => dest.StatusLabel,
                opt => opt.MapFrom(src => src.Status.GetDescription()))

            .ForMember(dest => dest.OrderTypeLabel,
                opt => opt.MapFrom(src => src.OrderType.GetDescription()))

            .ForMember(dest => dest.CancelReasonLabel,
                opt => opt.MapFrom(src => src.CancelReason.HasValue
                    ? src.CancelReason.Value.GetDescription()
                    : null))

            // 3. Các trường thông tin khác (Map từ Object con)
            .ForMember(dest => dest.StoreName,
                opt => opt.MapFrom(src => src.Store.Name))

            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.Username : "Khách vãng lai"))

            .ForMember(dest => dest.ShipperName,
                opt => opt.MapFrom(src => src.Shipper != null ? src.Shipper.Username : null))
            .ForMember(dest => dest.ShipperPhone,
                opt => opt.MapFrom(src => src.Shipper != null ? src.Shipper.Phone : null))

            .ForMember(dest => dest.TableName,
                opt => opt.MapFrom(src => src.Table != null ? src.Table.Name : null))

            .ForMember(dest => dest.PaymentMethodName,
                opt => opt.MapFrom(src =>
                    src.PaymentMethodName ?? (src.PaymentMethod != null ? src.PaymentMethod.Name : "Chưa chọn")))

            // 4. Logic tính toán trạng thái thanh toán
            .ForMember(dest => dest.IsPaid,
                opt => opt.MapFrom(src =>
                    src.OrderPayments.Any(p => p.Status == OrderPaymentStatusEnum.Paid)
                ))
            .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src =>
                src.OrderPayments != null
                ? src.OrderPayments.Where(p => p.Status == OrderPaymentStatusEnum.Paid).Sum(p => p.Amount)
                : 0
            ))
            // 5. Lọc Items (Chỉ lấy món chính, topping đã nằm trong món chính)
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom(src => src.OrderItems.Where(i => i.ParentItemId == null)));
        CreateMap<BaseOrderCreateDto, Order>()
            .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
            .ForMember(dest => dest.PaymentMethodId, opt => opt.MapFrom(src => src.PaymentMethodId))
            .ForMember(dest => dest.UserNotes, opt => opt.MapFrom(src => src.UserNotes))
            .ForMember(dest => dest.VoucherCodeUsed, opt => opt.MapFrom(src => src.VoucherCode))

            // Service tự xử lý
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.GrandTotal, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.OrderCode, opt => opt.Ignore());

        // 1. Tại quầy
        CreateMap<AtCounterOrderCreateDto, Order>()
            .IncludeBase<BaseOrderCreateDto, Order>() // Kế thừa logic map của Base
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.TableId))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => OrderTypeEnum.AtCounter)); // Gán cứng loại đơn

        // 2. Giao hàng
        CreateMap<DeliveryOrderCreateDto, Order>()
            .IncludeBase<BaseOrderCreateDto, Order>() // Kế thừa logic map của Base
            .ForMember(dest => dest.DeliveryAddressId, opt => opt.MapFrom(src => src.DeliveryAddressId))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => OrderTypeEnum.Delivery)); // Gán cứng loại đơn

        // --- OrderItem Mappings ---
        CreateMap<OrderItemCreateDto, OrderItem>()
             .ForMember(dest => dest.OrderId, opt => opt.Ignore())
             .ForMember(dest => dest.BasePrice, opt => opt.Ignore())
             .ForMember(dest => dest.FinalPrice, opt => opt.Ignore())
             .ForMember(dest => dest.ParentItem, opt => opt.Ignore())
             .ForMember(dest => dest.ParentItemId, opt => opt.Ignore())
             .ForMember(dest => dest.SugarLevel, opt => opt.MapFrom(src => src.SugarLevel))
             .ForMember(dest => dest.IceLevel, opt => opt.MapFrom(src => src.IceLevel));
        CreateMap<OrderItem, OrderItemReadDto>()
            .ForMember(dest => dest.UnitPrice,
                opt => opt.MapFrom(src =>
                    src.Quantity > 0 ? src.FinalPrice / src.Quantity : 0))
            .ForMember(dest => dest.TotalPrice,
                opt => opt.MapFrom(src => src.FinalPrice))
            .ForMember(dest => dest.SizeName,
                opt => opt.MapFrom(src => src.SizeName))
            .ForMember(dest => dest.SugarLevel,
                opt => opt.MapFrom(src => src.SugarLevel.ToString()))
            .ForMember(dest => dest.IceLevel,
                opt => opt.MapFrom(src => src.IceLevel.ToString()))
            .ForMember(dest => dest.Toppings,
                opt => opt.MapFrom(src => src.InverseParentItem));

        CreateMap<OrderItem, OrderToppingReadDto>()
            .ForMember(dest => dest.ProductId,
                opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.BasePrice,
                opt => opt.MapFrom(src => src.BasePrice))
            .ForMember(dest => dest.FinalPrice,
                opt => opt.MapFrom(src => src.FinalPrice));



        // --- OrderPayment Mappings ---
        CreateMap<OrderPayment, OrderPaymentReadDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PaymentMethodId,
                opt => opt.MapFrom(src => src.PaymentMethodId))
            .ForMember(dest => dest.PaymentMethodName,
                opt => opt.MapFrom(src => src.PaymentMethodName))
            .ForMember(dest => dest.Amount,
                opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.TransactionCode,
                opt => opt.MapFrom(src => src.TransactionCode))
            .ForMember(dest => dest.PaymentDate,
                opt => opt.MapFrom(src => src.PaymentDate))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));

        // --- PaymentMethod Mappings ---
        CreateMap<PaymentMethodCreateDto, PaymentMethod>().ReverseMap();
        CreateMap<PaymentMethod, PaymentMethodReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType.ToString()));
        CreateMap<PaymentMethodUpdateDto, PaymentMethod>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Payslip Mappings ---
        CreateMap<PayslipCreateDto, Payslip>().ReverseMap();
        CreateMap<Payslip, PayslipReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff.FullName))
            .ForMember(dest => dest.StaffPosition, opt => opt.MapFrom(src => src.Staff.Position.ToString()));
        CreateMap<PayslipUpdateDto, Payslip>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Policy Mappings ---
        CreateMap<PolicyCreateDto, Policy>()
             .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Title.ToLower().Replace(" ", "-")))
             .ReverseMap();
        CreateMap<Policy, PolicyReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null));
        CreateMap<PolicyUpdateDto, Policy>()
            .ForMember(dest => dest.Slug, opt => opt.Condition((src, dest) => src.Slug != null || src.Title != null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // =======================================================
        // 1. MAP CHO PRODUCT SIZE (QUAN TRỌNG)
        // =======================================================

        // A. Map từ Entity -> ReadDto (Hiển thị ra ngoài)
        CreateMap<ProductSize, ProductSizeReadDto>()
            .ForMember(dest => dest.SizeLabel, opt => opt.MapFrom(src => src.Size.Label))
            .ForMember(dest => dest.SizeModifierPrice, opt => opt.MapFrom(src => src.Size.PriceModifier ?? 0))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            // FinalPrice sẽ được tính toán sau (vì cần BasePrice của cha)
            .ForMember(dest => dest.FinalPrice, opt => opt.Ignore());
        // B. Map từ CreateDto -> Entity (Lưu vào DB)
        CreateMap<ProductSizeCreateDto, ProductSize>()
            .ForMember(dest => dest.SizeId, opt => opt.MapFrom(src => src.SizeId))
            .ForMember(dest => dest.PriceOverride, opt => opt.MapFrom(src => src.PriceOverride))
            // Mặc định Status là Active khi tạo mới, hoặc bạn có thể map nếu Dto có
            .ForMember(dest => dest.Status, opt => opt.Ignore());
        // 2. MAP CHO PRODUCT (CẬP NHẬT)
        // =======================================================

        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            // Map danh sách ProductSizes từ DTO sang ICollection<ProductSize>
            .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => src.ProductSizes))
            .ReverseMap();

        CreateMap<Product, ProductReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType.ToString()))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => src.ProductSizes))
            // 🔥 TÍNH TOÁN GIÁ CUỐI CÙNG (FinalPrice)
            .AfterMap((src, dest) =>
            {
                foreach (var sizeDto in dest.ProductSizes)
                {
                    // Logic: Nếu có PriceOverride thì dùng, nếu không thì lấy Giá gốc + Giá Size
                    if (sizeDto.PriceOverride.HasValue)
                    {
                        sizeDto.FinalPrice = sizeDto.PriceOverride.Value;
                    }
                    else
                    {
                        sizeDto.FinalPrice = src.BasePrice + sizeDto.SizeModifierPrice;
                    }
                }
            });

        CreateMap<ProductUpdateDto, Product>()
             .ForMember(dest => dest.Slug, opt => opt.Condition(src => src.Slug != null))
             .ForMember(dest => dest.ProductSizes, opt => opt.Ignore()) // Update Size xử lý riêng trong Service
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // =======================================================
        // 3. MAP CHO PRODUCT STORE (NẾU DÙNG)
        // =======================================================
        CreateMap<ProductStore, ProductStoreReadDto>()
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        // Trong Constructor của MappingProfile:

        CreateMap<Product, StoreMenuReadDto>()
            // Copy các map cơ bản từ ProductReadDto (hoặc dùng IncludeBase nếu cấu hình chuẩn)
            .IncludeBase<Product, ProductReadDto>()
            .ForMember(dest => dest.IsSoldOut, opt => opt.Ignore()) // Tính toán trong Service
            .ForMember(dest => dest.StoreStatus, opt => opt.Ignore()) // Tính toán trong Service
            .ForMember(dest => dest.DisplayPrice, opt => opt.Ignore()); // Tính toán trong Service


        // --- Reservation Mappings ---
        CreateMap<ReservationCreateDto, Reservation>()
             .ForMember(dest => dest.ReservationCode, opt => opt.Ignore())
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReservationStatusEnum.Pending))
             .ForMember(dest => dest.AssignedTableId, opt => opt.Ignore())
             .ReverseMap();
        CreateMap<Reservation, ReservationReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
            .ForMember(dest => dest.AssignedTableName, opt => opt.MapFrom(src => src.AssignedTable != null ? src.AssignedTable.Name : null));
        CreateMap<ReservationUpdateDto, Reservation>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Review Mappings ---

        // 1. Create: Map từ DTO tạo mới sang Entity
        CreateMap<ReviewCreateDto, Review>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId lấy từ Token trong Service
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReviewStatusEnum.Pending)) // Mặc định là Chờ duyệt
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => false)) // Mặc định chưa chỉnh sửa
                                                                                // OrderId, ProductId, Rating, Content, MediaUrl sẽ tự map do trùng tên
            .ReverseMap();

        // 2. Read: Map từ Entity sang DTO hiển thị
        CreateMap<Review, ReviewReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) // Enum -> String

            // Flattening dữ liệu Sản phẩm
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrl))

            // Flattening dữ liệu User
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username)) // Hoặc FullName tùy bạn
            .ForMember(dest => dest.UserThumbnailUrl, opt => opt.MapFrom(src => src.User.ThumbnailUrl));

        // 3. Update (User): Khách hàng sửa bài đánh giá
        CreateMap<ReviewUserEditDto, Review>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        // Chỉ update những trường có gửi lên (Rating, Content, MediaUrl), null thì giữ nguyên

        // 4. Update (Admin): Admin duyệt hoặc trả lời
        CreateMap<ReviewAdminUpdateDto, Review>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        // --- Room Mappings ---
        CreateMap<RoomCreateDto, Room>().ReverseMap();
        CreateMap<Room, RoomReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.TotalTables, opt => opt.MapFrom(src => src.ShopTables.Count));
        CreateMap<RoomUpdateDto, Room>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- ShopTable Mappings ---
        CreateMap<ShopTableCreateDto, ShopTable>().ReverseMap();
        CreateMap<ShopTable, ShopTableReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.MergedWithTableName, opt => opt.MapFrom(src => src.MergedWithTable != null ? src.MergedWithTable.Name : null))
            .ForMember(dest => dest.MergedTables, opt => opt.MapFrom(src => src.InverseMergedWithTable))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room != null ? src.Room.Name : null));
        CreateMap<ShopTableUpdateDto, ShopTable>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Size Mappings ---
        CreateMap<Size, SizeReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<SizeCreateDto, Size>().ReverseMap();
        CreateMap<SizeUpdateDto, Size>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- SocialMedia Mappings ---
        CreateMap<SocialMediaCreateDto, SocialMedia>().ReverseMap();
        CreateMap<SocialMedia, SocialMediaReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : string.Empty))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null));
        CreateMap<SocialMediaUpdateDto, SocialMedia>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Staff Mappings ---
        CreateMap<StaffCreateDto, Staff>()
            .ForMember(dest => dest.PublicId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PublicStatusEnum.Active))
            .ReverseMap();
        CreateMap<Staff, StaffReadDto>()
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position.ToString()))
            .ForMember(dest => dest.SalaryType, opt => opt.MapFrom(src => src.SalaryType.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.User.ThumbnailUrl))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : "Trụ sở chính"));
        CreateMap<StaffUpdateDto, Staff>()
            .ForMember(dest => dest.PublicId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- Store Mappings ---
        CreateMap<StoreCreateDto, Store>()
             .ForMember(dest => dest.Slug, opt => opt.Ignore())
             .ReverseMap();
        CreateMap<Store, StoreReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        CreateMap<StoreUpdateDto, Store>()
             .ForMember(dest => dest.Slug, opt => opt.Condition(src => src.Slug != null))
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- SupplyOrder Mappings ---
        CreateMap<SupplyOrderCreateDto, SupplyOrder>()
            .ForMember(dest => dest.SupplyOrderItems, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.OrderCode, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.PublicId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<SupplyOrderItemCreateDto, SupplyOrderItem>()
            .ForMember(dest => dest.Unit, opt => opt.Ignore())
            .ForMember(dest => dest.CostPerUnit, opt => opt.Ignore())
            .ForMember(dest => dest.TotalCost, opt => opt.Ignore());
        CreateMap<SupplyOrder, SupplyOrderReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : "Kho Tổng (HQ)"))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.Username : null))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SupplyOrderItems));
        CreateMap<SupplyOrderItem, SupplyOrderItemReadDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material.Name))
            .ForMember(dest => dest.MaterialImageUrl, opt => opt.MapFrom(src => src.Material.ImageUrl));
        CreateMap<SupplyOrderUpdateDto, SupplyOrder>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- User Mappings ---
        CreateMap<UserRegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => UserRoleEnum.Customer))
            .ReverseMap();
        CreateMap<User, UserReadDto>()
            // Map Role (Số)
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.RoleId))
            // Map Role Label (Chữ tiếng Việt)
            .ForMember(dest => dest.RoleLabel, opt => opt.MapFrom(src => src.RoleId.GetDescription()))
            // Map Status (Số)
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            // Map Status Label (Chữ tiếng Việt)
            .ForMember(dest => dest.StatusLabel, opt => opt.MapFrom(src => src.Status.GetDescription()));
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- VoucherTemplate Mappings ---
        CreateMap<VoucherTemplateCreateDto, VoucherTemplate>().ReverseMap();
        CreateMap<VoucherTemplate, VoucherTemplateReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.MembershipLevel != null ? src.MembershipLevel.Name : "Tất cả"));
        CreateMap<VoucherTemplateUpdateDto, VoucherTemplate>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // --- UserVoucher Mappings ---
        CreateMap<UserVoucherCreateDto, UserVoucher>()
             .ForMember(dest => dest.VoucherCode, opt => opt.Condition(src => src.VoucherCode != null))
             .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserVoucherStatusEnum.Unused))
             .ForMember(dest => dest.UsedDate, opt => opt.Ignore())
             .ForMember(dest => dest.OrderIdUsed, opt => opt.Ignore())
             .ReverseMap();
        CreateMap<UserVoucher, UserVoucherReadDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TemplateName, opt => opt.MapFrom(src => src.VoucherTemplate.Name))
            .ForMember(dest => dest.DiscountValue, opt => opt.MapFrom(src => src.VoucherTemplate.DiscountValue))
            .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.VoucherTemplate.DiscountType))
            .ForMember(dest => dest.MinOrderValue, opt => opt.MapFrom(src => src.VoucherTemplate.MinOrderValue))
            .ForMember(dest => dest.MaxDiscountAmount, opt => opt.MapFrom(src => src.VoucherTemplate.MaxDiscountAmount));
    }
}