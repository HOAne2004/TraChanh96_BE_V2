using drinking_be.Enums;
using drinking_be.Models;
using drinking_be.Utils;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(DBDrinkContext context)
        {
            // --- 1. Membership Levels ---
            if (!await context.MembershipLevels.AnyAsync())
            {
                context.MembershipLevels.AddRange(
                    new MembershipLevel { Name = "Đồng", MinSpendRequired = 0m, DurationDays = 30, CreatedAt = DateTime.UtcNow, Benefits = "{}" },
                    new MembershipLevel { Name = "Bạc", MinSpendRequired = 280000m, DurationDays = 30, CreatedAt = DateTime.UtcNow, Benefits = "{}" },
                    new MembershipLevel { Name = "Vàng", MinSpendRequired = 600000m, DurationDays = 30, CreatedAt = DateTime.UtcNow, Benefits = "{}" },
                    new MembershipLevel { Name = "Kim Cương", MinSpendRequired = 1000000m, DurationDays = 30, CreatedAt = DateTime.UtcNow, Benefits = "{}" }
                );
                await context.SaveChangesAsync();
            }

            // --- 2. Admin User & Staff ---
            if (!await context.Users.AnyAsync(u => u.Email == "admin@drink.vn"))
            {
                var baseLevel = await context.MembershipLevels.FirstOrDefaultAsync(l => l.Name == "Đồng");

                // Tạo User Admin
                var adminUser = new User
                {
                    PublicId = Guid.NewGuid(),
                    Username = "Admin",
                    Email = "admin@drink.vn",
                    PasswordHash = PasswordHasher.HashPassword("Admin@123"),
                    RoleId = UserRoleEnum.Admin, // Sử dụng Enum mới
                    Status = UserStatusEnum.Active, // Sử dụng Enum mới
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                // Tạo Staff Profile cho Admin (Nếu hệ thống yêu cầu Admin cũng là Staff)
                // Hoặc bỏ qua nếu Admin không cần nằm trong bảng Staff. 
                // Nhưng tốt nhất nên tạo để nhất quán logic "Lấy thông tin nhân viên".
                var adminStaff = new Staff
                {
                    UserId = adminUser.Id,
                    FullName = "Quản trị viên hệ thống",
                    Position = StaffPositionEnum.OfficeStaff, // Vị trí văn phòng
                    SalaryType = SalaryTypeEnum.FullTime,
                    StoreId = null, // Admin HQ không thuộc Store nào
                    HireDate = DateTime.UtcNow,
                    Status = PublicStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                };
                context.Staffs.Add(adminStaff);
                await context.SaveChangesAsync();

                // Tạo Membership cho Admin (để test tính năng mua hàng)
                if (baseLevel != null)
                {
                    context.Memberships.Add(new Membership
                    {
                        UserId = adminUser.Id,
                        LevelId = baseLevel.Id,
                        CardCode = $"ADM-{adminUser.Id}-{DateTime.UtcNow.Ticks}",
                        LevelStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        LevelEndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(baseLevel.DurationDays)
                    });
                    await context.SaveChangesAsync();
                }
            }

            // --- 2.1 Tạo Store (Cần có Store trước khi tạo Manager) ---
            if (!await context.Stores.AnyAsync())
            {
                if (!await context.Brands.AnyAsync())
                {
                    context.Brands.Add(new Brand { Name = "Trà Chanh 96", Status = PublicStatusEnum.Active });
                    await context.SaveChangesAsync();
                }

                var brand = await context.Brands.FirstOrDefaultAsync(b => b.Name == "Trà Chanh 96");
                if (brand != null)
                {
                    context.Stores.AddRange(
                       new Store
                       {
                           Name = "Trà Chanh 96 - Cầu Giấy",
                           Slug = "tc-cau-giay",
                           Address = new Address
                           {
                               RecipientName = "Quản lý Cầu Giấy",
                               RecipientPhone = "02431234567",
                               AddressDetail = "123 Đường Cầu Giấy",
                               Commune = "Quan Hoa",
                               District = "Cầu Giấy",
                               Province = "Hà Nội",
                               FullAddress = "123 Đường Cầu Giấy, Phường Quan Hoa, Quận Cầu Giấy, Hà Nội",
                               Latitude = 21.0357, // Tọa độ giả lập khu Cầu Giấy
                               Longitude = 105.8015,
                               IsDefault = true,
                               Status = PublicStatusEnum.Active,
                               CreatedAt = DateTime.UtcNow
                           },
                           BrandId = brand.Id,
                           OpenDate = new DateTime(2019, 11, 19),
                           OpenTime = new TimeSpan(8, 0, 0),
                           CloseTime = new TimeSpan(22, 0, 0),
                           ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=500&q=60",
                           ShippingFeeFixed = 10000,
                           Status = StoreStatusEnum.Active
                       },
                       new Store
                       {
                           Name = "Trà Chanh 96 - Đống Đa",
                           Slug = "tc-dong-da",
                           Address = new Address
                           {
                               RecipientName = "Quản lý Đống Đa",
                               RecipientPhone = "02439876543",
                               AddressDetail = "456 Xã Đàn",
                               Commune = "Nam Đồng",
                               District = "Đống Đa",
                               Province = "Hà Nội",
                               FullAddress = "456 Xã Đàn, Phường Nam Đồng, Quận Đống Đa, Hà Nội",
                               Latitude = 21.0163, // Tọa độ giả lập khu Xã Đàn
                               Longitude = 105.8364,
                               IsDefault = true,
                               Status = PublicStatusEnum.Active,
                               CreatedAt = DateTime.UtcNow
                           },
                           BrandId = brand.Id,
                           OpenDate = new DateTime(2021, 06, 07),
                           OpenTime = new TimeSpan(8, 0, 0),
                           CloseTime = new TimeSpan(23, 0, 0),
                           ImageUrl = "https://images.unsplash.com/photo-1559925393-8be0ec4767c8?auto=format&fit=crop&w=500&q=60",
                           ShippingFeeFixed = 15000,
                           Status = StoreStatusEnum.Active
                       }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // --- 3. Product Options (Sizes, Sugar, Ice) ---
            if (!await context.Sizes.AnyAsync())
            {
                context.Sizes.AddRange(
                    new Size { Label = "Nhỏ", PriceModifier = 0, Status = PublicStatusEnum.Active },
                    new Size { Label = "Vừa", PriceModifier = 5000, Status = PublicStatusEnum.Active },
                    new Size { Label = "Lớn", PriceModifier = 10000, Status = PublicStatusEnum.Active }
                );
                await context.SaveChangesAsync();
            }

            // --- 4. Categories ---
            if (!await context.Categories.AnyAsync())
            {
                context.Categories.AddRange(
                    new Category { Name = "Trà Trái Cây", Slug = "tra-trai-cay", Status = PublicStatusEnum.Active },
                    new Category { Name = "Trà Sữa", Slug = "tra-sua", Status = PublicStatusEnum.Active },
                    new Category { Name = "Cà Phê", Slug = "ca-phe", Status = PublicStatusEnum.Active },
                    new Category { Name = "Topping", Slug = "topping", Status = PublicStatusEnum.Active }
                );
                await context.SaveChangesAsync();
            }

            // --- 5. Products ---
            // Phần này giữ nguyên nếu Product không đổi cấu trúc nhiều
            if (!await context.Products.AnyAsync())
            {
                var cateTraSua = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "tra-sua");
                var cateTraicay = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "tra-trai-cay");

                if (cateTraSua != null && cateTraicay != null)
                {
                    var sizeIds = await context.Sizes.Select(s => s.Id).ToListAsync();

                    var products = new List<Product>
                    {
                        new Product
                        {
                            PublicId = Guid.NewGuid().ToString(),
                            Name = "Trà Sữa Trân Châu Đường Đen",
                            Slug = "ts-tran-chau-duong-den",
                            BasePrice = 35000,
                            CategoryId = cateTraSua.Id,
                            ProductType = "Beverage",
                            ImageUrl = "https://images.unsplash.com/photo-1558160074-4d7d8bdf4256?auto=format&fit=crop&w=500&q=60",
                            Description = "Hương vị đường đen thơm lừng kết hợp sữa tươi.",
                            Status = ProductStatusEnum.Active,
                            TotalSold = 150,
                            TotalRating = 4.8,
                            LaunchDateTime = DateTime.UtcNow
                        },
                        // ... thêm sản phẩm khác
                    };

                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();

                    // Gán Size
                    foreach (var p in products)
                    {
                        foreach (var sId in sizeIds)
                            context.ProductSizes.Add(new ProductSize { ProductId = p.Id, SizeId = sId });
                    }
                    await context.SaveChangesAsync();
                }
            }

            // --- 6. News ---
            // Cập nhật lại News dùng UserRoleEnum check
            if (!await context.News.AnyAsync())
            {
                var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@drink.vn");
                if (admin != null)
                {
                    context.News.AddRange(
                        new News
                        {
                            Title = "Khai trương chi nhánh mới",
                            Slug = "khai-truong-moi",
                            Content = "Nội dung...",
                            Type = "Tin tức",
                            Status = ContentStatusEnum.Published,
                            UserId = admin.Id,
                            PublishedDate = DateTime.UtcNow,
                            ThumbnailUrl = "https://example.com/img.jpg"
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}