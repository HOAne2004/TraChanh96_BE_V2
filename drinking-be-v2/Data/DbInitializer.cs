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
            if (!await context.Database.CanConnectAsync()) return;
            Console.WriteLine("🔄 Bắt đầu Seed Data...");

            // --- 1. Brand (PHẢI tạo đầu tiên, vì Store cần Brand) ---
            if (!await context.Brands.AnyAsync())
            {
                context.Brands.Add(new Brand
                {
                    Name = "Trà Chanh 96",
                    Address = "Cụm CN Bình Lục, Bình An, Ninh Bình, Việt Nam",
                    Hotline = "0243 123 4567",
                    EmailSupport = "trachanh96@support.com",
                    TaxCode = "0101234567",
                    Status = PublicStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
                Console.WriteLine("--> Đã tạo Brand");
            }

            // --- 2. Membership Levels ---
            if (!await context.MembershipLevels.AnyAsync())
            {
                context.MembershipLevels.AddRange(
                    new MembershipLevel
                    {
                        Name = "Đồng",
                        MinCoinsRequired = 0,
                        DurationDays = 30,
                        RankOrder = 1, // 👈 Thêm dòng này
                        CreatedAt = DateTime.UtcNow,
                        Benefits = "{}"
                    },
                    new MembershipLevel
                    {
                        Name = "Bạc",
                        MinCoinsRequired = 350,
                        DurationDays = 30,
                        RankOrder = 2, // 👈 Giá trị phải khác nhau
                        CreatedAt = DateTime.UtcNow,
                        Benefits = "{}"
                    },
                    new MembershipLevel
                    {
                        Name = "Vàng",
                        MinCoinsRequired = 1000,
                        DurationDays = 30,
                        RankOrder = 3, // 👈
                        CreatedAt = DateTime.UtcNow,
                        Benefits = "{}"
                    },
                    new MembershipLevel
                    {
                        Name = "Kim Cương",
                        MinCoinsRequired = 2000,
                        DurationDays = 30,
                        RankOrder = 4, // 👈
                        CreatedAt = DateTime.UtcNow,
                        Benefits = "{}"
                    }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("--> Đã tạo Membership Levels");
            }

            // --- 3. Admin User & Staff ---
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

                // Tạo Staff Profile cho Admin
                context.Staffs.Add(new Staff
                {
                    UserId = adminUser.Id, // Đã có ID từ bước trên
                    FullName = "Quản trị viên hệ thống",
                    Position = StaffPositionEnum.OfficeStaff,
                    SalaryType = SalaryTypeEnum.FullTime,
                    HireDate = DateTime.UtcNow,
                    Status = PublicStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                });

                // Tạo Customer Test
                context.Users.Add(new User
                {
                    PublicId = Guid.NewGuid(),
                    Username = "khachhang1",
                    Email = "khach@email.com",
                    PasswordHash = PasswordHasher.HashPassword("123456"),
                    RoleId = UserRoleEnum.Customer,
                    Status = UserStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync(); // 💾 LƯU STAFF & CUSTOMER
                Console.WriteLine("--> Đã tạo Users (Admin & Customer)");

                // Tạo Membership cho Admin (để test tính năng mua hàng)
                if (baseLevel != null)
                {
                    context.Memberships.Add(new Membership
                    {
                        UserId = adminUser.Id,
                        MembershipLevelId = baseLevel.Id,
                        CardCode = $"ADM-{adminUser.Id}-{DateTime.UtcNow.Ticks}",
                        LevelStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        LevelEndDate = baseLevel.DurationDays.HasValue
                            ? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(baseLevel.DurationDays.Value)
                            : null
                    });
                    await context.SaveChangesAsync();
                }
            }
            // Tạo User Thu ngân
            if (!await context.Users.AnyAsync(u => u.Email == "thungan@drink.vn"))
            {
                var cashierUser = new User
                {
                    PublicId = Guid.NewGuid(),
                    Username = "thungan_cg",
                    Email = "thungan@drink.vn",
                    PasswordHash = PasswordHasher.HashPassword("123456"),
                    RoleId = UserRoleEnum.Staff,
                    Status = UserStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(cashierUser);
                await context.SaveChangesAsync();

                // Lấy Store ID (giả sử Cầu Giấy đã tạo hoặc hardcode logic lấy store)
                var store = await context.Stores.FirstOrDefaultAsync(s => s.Slug == "tc-cau-giay");

                context.Staffs.Add(new Staff
                {
                    UserId = cashierUser.Id,
                    FullName = "Trần Thị Thu Ngân",
                    Position = StaffPositionEnum.Cashier, // Cần đảm bảo Enum này có giá trị
                    StoreId = store?.Id, // Gán nhân viên vào cửa hàng cụ thể
                    SalaryType = SalaryTypeEnum.PartTime,
                    Status = PublicStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Tạo User Pha chế
            if (!await context.Users.AnyAsync(u => u.Email == "phache@drink.vn"))
            {
                var baristaUser = new User
                {
                    PublicId = Guid.NewGuid(),
                    Username = "phache_cg",
                    Email = "phache@drink.vn",
                    PasswordHash = PasswordHasher.HashPassword("123456"),
                    RoleId = UserRoleEnum.Staff,
                    Status = UserStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(baristaUser);
                await context.SaveChangesAsync();

                var store = await context.Stores.FirstOrDefaultAsync(s => s.Slug == "tc-cau-giay");

                context.Staffs.Add(new Staff
                {
                    UserId = baristaUser.Id,
                    FullName = "Nguyễn Văn Pha Chế",
                    Position = StaffPositionEnum.Barista, // Barista/Kitchen
                    StoreId = store?.Id,
                    SalaryType = SalaryTypeEnum.FullTime,
                    Status = PublicStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
                Console.WriteLine("--> Đã tạo Staff (Cashier & Barista)");
            }

            // --- 4. Store (sau khi đã có Brand) ---
            if (!await context.Stores.AnyAsync())
            {
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
                               UserId = null, // Store address không cần UserId
                               RecipientName = "Quản lý Cầu Giấy",
                               RecipientPhone = "02431234567",
                               AddressDetail = "123 Đường Cầu Giấy",
                               Commune = "Quan Hoa",
                               District = "Cầu Giấy",
                               Province = "Hà Nội",
                               FullAddress = "123 Đường Cầu Giấy, Phường Quan Hoa, Quận Cầu Giấy, Hà Nội",
                               Latitude = 21.0357,
                               Longitude = 105.8015,
                               IsDefault = true,
                               Status = PublicStatusEnum.Active,
                               CreatedAt = DateTime.UtcNow,
                               UpdatedAt = DateTime.UtcNow
                           },
                           BrandId = brand.Id,
                           OpenDate = new DateTime(2019, 11, 19),
                           OpenTime = new TimeSpan(8, 0, 0),
                           CloseTime = new TimeSpan(22, 0, 0),
                           ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=500&q=60",
                           ShippingFeeFixed = 10000,
                           Status = StoreStatusEnum.Active,
                           CreatedAt = DateTime.UtcNow,
                           UpdatedAt = DateTime.UtcNow
                       },
                       new Store
                       {
                           Name = "Trà Chanh 96 - Đống Đa",
                           Slug = "tc-dong-da",
                           Address = new Address
                           {
                               UserId = null,
                               RecipientName = "Quản lý Đống Đa",
                               RecipientPhone = "02439876543",
                               AddressDetail = "456 Xã Đàn",
                               Commune = "Nam Đồng",
                               District = "Đống Đa",
                               Province = "Hà Nội",
                               FullAddress = "456 Xã Đàn, Phường Nam Đồng, Quận Đống Đa, Hà Nội",
                               Latitude = 21.0163,
                               Longitude = 105.8364,
                               IsDefault = true,
                               Status = PublicStatusEnum.Active,
                               CreatedAt = DateTime.UtcNow,
                               UpdatedAt = DateTime.UtcNow
                           },
                           BrandId = brand.Id,
                           OpenDate = new DateTime(2021, 6, 7),
                           OpenTime = new TimeSpan(8, 0, 0),
                           CloseTime = new TimeSpan(23, 0, 0),
                           ImageUrl = "https://images.unsplash.com/photo-1559925393-8be0ec4767c8?auto=format&fit=crop&w=500&q=60",
                           ShippingFeeFixed = 15000,
                           Status = StoreStatusEnum.Active,
                           CreatedAt = DateTime.UtcNow,
                           UpdatedAt = DateTime.UtcNow
                       },
                       new Store
                       {
                           Name = "Trà Chanh 96 - Bình An",
                           Slug = "tc-binh-an",
                           Address = new Address
                           {
                               UserId = null,
                               RecipientName = "Quản lý Bình An",
                               RecipientPhone = "02439876543",
                               AddressDetail = "Cụm CN Bình Lục",
                               Commune = "",
                               District = "Bình An",
                               Province = "Ninh Bình",
                               FullAddress = "Cụm CN Bình Lục, Bình An, Ninh Bình",
                               Latitude = 20.475025,
                               Longitude = 106.039162,
                               IsDefault = true,
                               Status = PublicStatusEnum.Active,
                               CreatedAt = DateTime.UtcNow,
                               UpdatedAt = DateTime.UtcNow
                           },
                           BrandId = brand.Id,
                           OpenDate = new DateTime(2019, 3, 2),
                           OpenTime = new TimeSpan(0, 0, 0),
                           CloseTime = new TimeSpan(0, 0, 0),
                           ImageUrl = "https://images.unsplash.com/photo-1559925393-8be0ec4767c8?auto=format&fit=crop&w=500&q=60",
                           ShippingFeeFixed = 15000,
                           Status = StoreStatusEnum.Active,
                           CreatedAt = DateTime.UtcNow,
                           UpdatedAt = DateTime.UtcNow
                       }

                    );
                    await context.SaveChangesAsync();
                    Console.WriteLine("--> Đã tạo Stores");
                }
            }

            // --- 5. Product Options (Sizes, Sugar, Ice) ---
            if (!await context.Sizes.AnyAsync())
            {
                context.Sizes.AddRange(
                    new Size { Label = "Nhỏ", PriceModifier = 0, Status = PublicStatusEnum.Active },
                    new Size { Label = "Vừa", PriceModifier = 5000, Status = PublicStatusEnum.Active },
                    new Size { Label = "Lớn", PriceModifier = 10000, Status = PublicStatusEnum.Active }
                );
                await context.SaveChangesAsync();
            }

            // --- 6. Categories ---
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

            /// --- 7. Products - Bổ sung đầy đủ hơn ---
            if (!await context.Products.AnyAsync())
            {
                var cateTraSua = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "tra-sua");
                var cateTraicay = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "tra-trai-cay");
                var cateCaPhe = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "ca-phe");
                var cateTopping = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "topping");

                if (cateTraSua != null && cateTraicay != null && cateCaPhe != null && cateTopping != null)
                {
                    var sizeIds = await context.Sizes.Select(s => s.Id).ToListAsync();

                    var products = new List<Product>
                     {
                         // === TRÀ SỮA ===
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅ BẮT BUỘC THÊM (Giả sử Brand mặc định ID = 1)
                             Name = "Trà Sữa Trân Châu Đường Đen",
                             Slug = "tra-sua-tran-chau-duong-den",
                             BasePrice = 35000,
                             CategoryId = cateTraSua.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1558160074-4d7d8bdf4256?auto=format&fit=crop&w=500&q=60",
                             Description = "Trà sữa thơm ngon với trân châu đường đen dẻo dai, vị ngọt thanh.",
                             Ingredient = "Trà đen, sữa tươi, trân châu đường đen, đường nâu",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 1250,
                             TotalRating = 4.8,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-6),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Trà Sữa Matcha",
                             Slug = "tra-sua-matcha",
                             BasePrice = 45000,
                             CategoryId = cateTraSua.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1561047029-3000c68339ca?auto=format&fit=crop&w=500&q=60",
                             Description = "Trà sữa matcha Nhật Bản nguyên chất, vị thanh mát, thơm hương trà xanh.",
                             Ingredient = "Bột matcha Nhật Bản, sữa tươi, đường, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 890,
                             TotalRating = 4.9,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-4),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Trà Sữa Oreo",
                             Slug = "tra-sua-oreo",
                             BasePrice = 40000,
                             CategoryId = cateTraSua.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1567306226416-28f0efdc88ce?auto=format&fit=crop&w=500&q=80",
                             Description = "Trà sữa kết hợp với bánh oreo nghiền nhỏ, vị béo ngậy đặc trưng.",
                             Ingredient = "Trà sữa, bánh oreo, kem cheese, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 650,
                             TotalRating = 4.7,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-3),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Trà Sữa Hokkaido",
                             Slug = "tra-sua-hokkaido",
                             BasePrice = 50000,
                             CategoryId = cateTraSua.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1563805042-7684c019e1cb?auto=format&fit=crop&w=500&q=60",
                             Description = "Trà sữa theo phong cách Hokkaido Nhật Bản với vị sữa béo đặc trưng.",
                             Ingredient = "Trà đen hảo hạng, sữa Hokkaido, đường nâu, kem sữa",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 420,
                             TotalRating = 4.6,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-2),
                             CreatedAt = DateTime.UtcNow
                         },

                         // === TRÀ TRÁI CÂY ===
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Trà Chanh 96",
                             Slug = "tra-chanh-96",
                             BasePrice = 25000,
                             CategoryId = cateTraicay.Id,
                             ProductType = ProductTypeEnum.Drink,
                             // 🖼️ ĐÃ SỬA ẢNH MỚI (Hình ly trà chanh rõ nét)
                             ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?auto=format&fit=crop&w=500&q=60",
                             Description = "Trà chanh truyền thống với vị chua ngọt thanh mát, hương thơm đặc trưng.",
                             Ingredient = "Trà đen, chanh tươi, đường, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 2100,
                             TotalRating = 4.9,
                             LaunchDateTime = DateTime.UtcNow.AddYears(-1),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Trà Đào Cam Sả",
                             Slug = "tra-dao-cam-sa",
                             BasePrice = 35000,
                             CategoryId = cateTraicay.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1563227812-0ea4c22e6cc8?auto=format&fit=crop&w=500&q=60",
                             Description = "Trà đào kết hợp cam tươi và sả, vị chua ngọt thanh mát, tốt cho sức khỏe.",
                             Ingredient = "Trà đen, đào tươi, cam, sả, đường, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 980,
                             TotalRating = 4.8,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-8),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Trà Vải",
                             Slug = "tra-vai",
                             BasePrice = 30000,
                             CategoryId = cateTraicay.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1544787219-7f47ccb76574?auto=format&fit=crop&w=500&q=60",
                             Description = "Trà vải thanh mát, vị ngọt tự nhiên từ vải tươi, hương thơm nhẹ nhàng.",
                             Ingredient = "Trà xanh, vải tươi, đường, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 720,
                             TotalRating = 4.7,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-5),
                             CreatedAt = DateTime.UtcNow
                         },

                         // === CÀ PHÊ ===
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Cà Phê Sữa Đá",
                             Slug = "ca-phe-sua-da",
                             BasePrice = 30000,
                             CategoryId = cateCaPhe.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1498804103079-a6351b050096?auto=format&fit=crop&w=500&q=60",
                             Description = "Cà phê phin truyền thống Việt Nam, sữa đặc thơm béo, đá mát lạnh.",
                             Ingredient = "Cà phê phin, sữa đặc, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 1800,
                             TotalRating = 4.8,
                             LaunchDateTime = DateTime.UtcNow.AddYears(-1),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Cà Phê Đen Đá",
                             Slug = "ca-phe-den-da",
                             BasePrice = 25000,
                             CategoryId = cateCaPhe.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1511537190424-bbbab87ac5eb?auto=format&fit=crop&w=500&q=60",
                             Description = "Cà phê đen nguyên chất, đậm đà hương vị, thích hợp cho người thích vị đắng.",
                             Ingredient = "Cà phê phin, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 1100,
                             TotalRating = 4.6,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-10),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Bạc Xỉu",
                             Slug = "bac-xiu",
                             BasePrice = 35000,
                             CategoryId = cateCaPhe.Id,
                             ProductType = ProductTypeEnum.Drink,
                             ImageUrl = "https://images.unsplash.com/photo-1572442388796-11668a67e53d?auto=format&fit=crop&w=500&q=60",
                             Description = "Cà phê sữa đặc biệt theo phong cách Sài Gòn, nhiều sữa ít cà phê.",
                             Ingredient = "Cà phê phin, sữa đặc, sữa tươi, đá",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 850,
                             TotalRating = 4.7,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-7),
                             CreatedAt = DateTime.UtcNow
                         },

                         // === TOPPING ===
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Trân Châu Trắng",
                             Slug = "tran-chau-trang",
                             BasePrice = 10000,
                             CategoryId = cateTopping.Id,
                             ProductType = ProductTypeEnum.Topping,
                             ImageUrl = "https://images.unsplash.com/photo-1572490122747-3968b75cc699?auto=format&fit=crop&w=500&q=60",
                             Description = "Trân châu trắng dẻo dai, vị ngọt thanh, ăn kèm với trà sữa.",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 3200,
                             TotalRating = 4.5,
                             LaunchDateTime = DateTime.UtcNow.AddYears(-1),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Thạch Cà Phê",
                             Slug = "thach-ca-phe",
                             BasePrice = 12000,
                             CategoryId = cateTopping.Id,
                             ProductType = ProductTypeEnum.Topping,
                             ImageUrl = "https://images.unsplash.com/photo-1567306226416-28f0efdc88ce?auto=format&fit=crop&w=500&q=80",
                             Description = "Thạch cà phê mềm mịn, vị đắng nhẹ hòa quyện với đồ uống.",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 1800,
                             TotalRating = 4.6,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-9),
                             CreatedAt = DateTime.UtcNow
                         },
                         new Product
                         {
                             PublicId = Guid.NewGuid(),
                             BrandId = 1, // ✅
                             Name = "Pudding Trứng",
                             Slug = "pudding-trung",
                             BasePrice = 15000,
                             CategoryId = cateTopping.Id,
                             ProductType = ProductTypeEnum.Topping,
                             ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=500&q=60",
                             Description = "Pudding trứng béo ngậy, mềm mịn, thơm hương vanilla.",
                             Status = ProductStatusEnum.Active,
                             TotalSold = 1450,
                             TotalRating = 4.8,
                             LaunchDateTime = DateTime.UtcNow.AddMonths(-6),
                             CreatedAt = DateTime.UtcNow
                         }
                     };
                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();

                    // Gán Size cho từng product (chỉ với đồ uống)
                    var drinkProducts = products.Where(p => p.ProductType == ProductTypeEnum.Drink).ToList();
                    foreach (var p in drinkProducts)
                    {
                        foreach (var sId in sizeIds)
                            context.ProductSizes.Add(new ProductSize { ProductId = p.Id, SizeId = sId });
                    }
                    await context.SaveChangesAsync();

                    // Gán sản phẩm vào các store
                    var stores = await context.Stores.ToListAsync();
                    var allProducts = await context.Products.ToListAsync();

                    foreach (var store in stores)
                    {
                        foreach (var product in allProducts)
                        {
                            // Random stock quantity từ 50-200
                            var random = new Random();
                            var soldCount = random.Next(50, 200);

                            context.ProductStores.Add(new ProductStore
                            {
                                StoreId = store.Id,
                                ProductId = product.Id,
                                SoldCount = soldCount,
                                Status = ProductStoreStatusEnum.Available,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("--> Đã tạo Products & Liên kết Store");
                }
            }

            // --- 8. News - Bổ sung nhiều tin tức hơn ---
            if (!await context.News.AnyAsync())
            {
                var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@drink.vn");
                if (admin != null)
                {
                    context.News.AddRange(
                        new News
                        {
                            Title = "Khai trương chi nhánh mới tại Quận Cầu Giấy",
                            Slug = "khai-truong-chi-nhanh-moi-cau-giay",
                            Content = @"<p>Chúng tôi vui mừng thông báo về việc khai trương chi nhánh mới tại số 123 Đường Cầu Giấy, Quận Cầu Giấy, Hà Nội.</p>
                          <p>Chi nhánh mới được thiết kế với không gian hiện đại, thoáng mát, phù hợp cho các buổi hẹn hò, làm việc nhóm hay thư giãn sau giờ làm.</p>
                          <p>Đặc biệt, trong tuần lễ khai trương (từ 01/12 đến 07/12), tất cả khách hàng đến cửa hàng sẽ được:</p>
                          <ul>
                            <li>Giảm 30% cho hóa đơn từ 100.000đ</li>
                            <li>Tặng kèm 1 topping bất kỳ cho đơn hàng đầu tiên</li>
                            <li>Tích điểm gấp đôi trên thẻ thành viên</li>
                          </ul>
                          <p>Hãy đến và trải nghiệm không gian mới cùng những món đồ uống chất lượng từ Trà Chanh 96!</p>",
                            Type = NewsTypeEnum.News,
                            Status = ContentStatusEnum.Published,
                            UserId = admin.Id,
                            ThumbnailUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=800&q=80",
                            ViewCount = 1250,
                            PublishedDate = DateTime.UtcNow.AddDays(-5),
                            CreatedAt = DateTime.UtcNow
                        },
                        new News
                        {
                            Title = "Giới thiệu dòng sản phẩm mới: Trà Sữa Hokkaido",
                            Slug = "gioi-thieu-tra-sua-hokkaido",
                            Content = @"<p>Tiếp nối thành công của các dòng sản phẩm trà sữa truyền thống, Trà Chanh 96 tự hào giới thiệu dòng sản phẩm mới: <strong>Trà Sữa Hokkaido</strong>.</p>
                          <p>Với nguyên liệu chính là sữa bột nguyên kem nhập khẩu từ vùng Hokkaido - Nhật Bản, cùng trà đen hảo hạng từ Đài Loan, Trà Sữa Hokkaido mang đến hương vị béo ngậy, thơm ngon khó cưỡng.</p>
                          <h3>Đặc điểm nổi bật:</h3>
                          <ul>
                            <li>Sữa bột nguyên kem từ Hokkaido, Nhật Bản</li>
                            <li>Trà đen hảo hạng, ủ đúng thời gian</li>
                            <li>Vị béo tự nhiên, không ngấy</li>
                            <li>Giàu canxi và vitamin D</li>
                          </ul>
                          <p>Sản phẩm hiện đã có mặt tại tất cả các chi nhánh của Trà Chanh 96 với giá chỉ từ 50.000đ.</p>",
                            Type = NewsTypeEnum.Announcement,
                            Status = ContentStatusEnum.Published,
                            UserId = admin.Id,
                            ThumbnailUrl = "https://images.unsplash.com/photo-1563805042-7684c019e1cb?auto=format&fit=crop&w=800&q=80",
                            ViewCount = 890,
                            PublishedDate = DateTime.UtcNow.AddDays(-10),
                            CreatedAt = DateTime.UtcNow
                        },
                        new News
                        {
                            Title = "Chương trình tích điểm đổi quà 2024",
                            Slug = "chuong-trinh-tich-diem-doi-qua-2024",
                            Content = @"<p>Từ ngày 01/01/2024, Trà Chanh 96 chính thức áp dụng chương trình tích điểm đổi quà mới với nhiều cải tiến và ưu đãi hấp dẫn.</p>
                          <h3>Cách thức tích điểm:</h3>
                          <ul>
                            <li>1.000đ = 1 điểm</li>
                            <li>Tích điểm gấp đôi vào thứ 4 hàng tuần</li>
                            <li>Tích điểm gấp 3 cho đơn hàng online</li>
                          </ul>
                          <h3>Quyền lợi thành viên:</h3>
                          <table border='1'>
                            <tr>
                              <th>Hạng thành viên</th>
                              <th>Điểm yêu cầu</th>
                              <th>Ưu đãi</th>
                            </tr>
                            <tr>
                              <td>Đồng</td>
                              <td>0 điểm</td>
                              <td>Giảm 5% cho đơn hàng thứ 3 trong tháng</td>
                            </tr>
                            <tr>
                              <td>Bạc</td>
                              <td>350 điểm</td>
                              <td>Giảm 10%, tặng sinh nhật</td>
                            </tr>
                            <tr>
                              <td>Vàng</td>
                              <td>1000 điểm</td>
                              <td>Giảm 15%, ưu tiên giao hàng</td>
                            </tr>
                            <tr>
                              <td>Kim Cương</td>
                              <td>2000 điểm</td>
                              <td>Giảm 20%, tặng quà VIP</td>
                            </tr>
                          </table>
                          <p>Đăng ký thẻ thành viên ngay hôm nay để không bỏ lỡ những ưu đãi đặc biệt!</p>",
                            Type = NewsTypeEnum.Promotion,
                            Status = ContentStatusEnum.Published,
                            UserId = admin.Id,
                            ThumbnailUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?auto=format&fit=crop&w=800&q=80",
                            ViewCount = 1560,
                            PublishedDate = DateTime.UtcNow.AddDays(-15),
                            CreatedAt = DateTime.UtcNow
                        },
                        new News
                        {
                            Title = "Công thức pha chế trà chanh chuẩn vị 96",
                            Slug = "cong-thuc-pha-che-tra-chanh",
                            Content = @"<p>Trà chanh là thức uống đơn giản nhưng không phải ai cũng pha được ly trà chanh ngon đúng điệu. Hôm nay, Trà Chanh 96 sẽ bật mí công thức pha chế trà chanh chuẩn vị của chúng tôi.</p>
                          <h3>Nguyên liệu cần chuẩn bị:</h3>
                          <ul>
                            <li>Trà đen: 5g</li>
                            <li>Nước sôi: 200ml</li>
                            <li>Chanh tươi: 1/2 quả</li>
                            <li>Đường: 20-30g (tùy khẩu vị)</li>
                            <li>Đá viên</li>
                          </ul>
                          <h3>Các bước thực hiện:</h3>
                          <ol>
                            <li>Pha trà: Cho 5g trà đen vào 200ml nước sôi, ủ trong 5 phút</li>
                            <li>Lọc trà: Lọc bỏ bã trà, để nguội tự nhiên</li>
                            <li>Pha chế: Cho đường vào ly, vắt chanh, khuấy đều</li>
                            <li>Thêm trà: Đổ trà đã nguội vào ly, khuấy đều</li>
                            <li>Thêm đá: Cho đá viên vào và thưởng thức</li>
                          </ol>
                          <p>Mẹo nhỏ: Để trà chanh thơm hơn, có thể thêm vài lá bạc hà hoặc vài lát gừng.</p>
                          <p>Chúc các bạn thành công với công thức này!</p>",
                            Type = NewsTypeEnum.News,
                            Status = ContentStatusEnum.Published,
                            UserId = admin.Id,
                            ThumbnailUrl = "https://images.unsplash.com/photo-1622921491196-0e2fdf0d8c31?auto=format&fit=crop&w=800&q=80",
                            ViewCount = 3200,
                            PublishedDate = DateTime.UtcNow.AddDays(-20),
                            CreatedAt = DateTime.UtcNow
                        },
                        new News
                        {
                            Title = "Tuyển dụng nhân viên pha chế Full-time/Part-time",
                            Slug = "tuyen-dung-nhan-vien-pha-che",
                            Content = @"<p>Với sự phát triển mở rộng hệ thống, Trà Chanh 96 cần tuyển dụng thêm nhân viên pha chế cho các chi nhánh tại Hà Nội.</p>
                          <h3>Thông tin tuyển dụng:</h3>
                          <ul>
                            <li><strong>Vị trí:</strong> Nhân viên pha chế</li>
                            <li><strong>Số lượng:</strong> 10 người</li>
                            <li><strong>Hình thức:</strong> Full-time/Part-time</li>
                            <li><strong>Địa điểm làm việc:</strong> Các chi nhánh tại Hà Nội</li>
                          </ul>
                          <h3>Yêu cầu:</h3>
                          <ul>
                            <li>Tuổi: 18-30</li>
                            <li>Có kinh nghiệm pha chế là lợi thế (không yêu cầu với ứng viên part-time)</li>
                            <li>Nhanh nhẹn, nhiệt tình, có tinh thần trách nhiệm</li>
                            <li>Khả năng giao tiếp tốt</li>
                          </ul>
                          <h3>Quyền lợi:</h3>
                          <ul>
                            <li>Lương: 5-8 triệu/tháng (part-time), 8-12 triệu/tháng (full-time)</li>
                            <li>Được đào tạo bài bản về pha chế</li>
                            <li>Môi trường làm việc trẻ trung, năng động</li>
                            <li>Hưởng đầy đủ chế độ BHXH, BHYT, BHTN (full-time)</li>
                            <li>Ưu đãi 50% khi mua đồ uống tại cửa hàng</li>
                          </ul>
                          <p>Ứng viên quan tâm vui lòng gửi CV về email: tuyendung@trachanh96.vn</p>",
                            Type = NewsTypeEnum.Announcement,
                            Status = ContentStatusEnum.Published,
                            UserId = admin.Id,
                            ThumbnailUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?auto=format&fit=crop&w=800&q=80",
                            ViewCount = 2100,
                            PublishedDate = DateTime.UtcNow.AddDays(-25),
                            CreatedAt = DateTime.UtcNow
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // --- 9. Banners (Nếu có bảng Banner) ---
            if (!await context.Banners.AnyAsync())
            {
                context.Banners.AddRange(
                    new Banner
                    {
                        ImageUrl = "https://images.unsplash.com/photo-1567306226416-28f0efdc88ce?auto=format&fit=crop&w=1200&q=80",
                        Title = "Trà Sữa Mới - Ưu Đãi 30%",
                        LinkUrl = "/products/tra-sua-hokkaido",
                        SortOrder = 1,
                        Position = "Home-Top",
                        Status = PublicStatusEnum.Active,
                        StartAt = DateTime.UtcNow.AddDays(-7),
                        EndAt = DateTime.UtcNow.AddDays(7),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Banner
                    {
                        ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=1200&q=80",
                        Title = "Khai Trương Chi Nhánh Mới",
                        LinkUrl = "/news/khai-truong-chi-nhanh-moi-cau-giay",
                        SortOrder = 2,
                        Position = "Home-Top",
                        Status = PublicStatusEnum.Active,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Banner
                    {
                        ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?auto=format&fit=crop&w-800&q=80",
                        Title = "Tích Điểm Đổi Quà",
                        LinkUrl = "/membership",
                        SortOrder = 3,
                        Position = "Home-Middle",
                        Status = PublicStatusEnum.Active,
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            // --- 10. Test Users & Orders (cho demo) ---
            if (!await context.Users.AnyAsync(u => u.RoleId == UserRoleEnum.Customer))
            {
                // Tạo vài user khách hàng
                var customers = new List<User>
                {
                    new User
                    {
                        PublicId = Guid.NewGuid(),
                        Username = "customer1",
                        Email = "customer1@email.com",
                        Phone = "0912345678",
                        PasswordHash = PasswordHasher.HashPassword("Customer@123"),
                        RoleId = UserRoleEnum.Customer,
                        Status = UserStatusEnum.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        PublicId = Guid.NewGuid(),
                        Username = "customer2",
                        Email = "customer2@email.com",
                        Phone = "0923456789",
                        PasswordHash = PasswordHasher.HashPassword("Customer@123"),
                        RoleId = UserRoleEnum.Customer,
                        Status = UserStatusEnum.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.Users.AddRange(customers);
                await context.SaveChangesAsync();

                // Tạo địa chỉ cho khách hàng
                var address = new Address
                {
                    UserId = customers[0].Id,
                    RecipientName = "Nguyễn Văn A",
                    RecipientPhone = "0912345678",
                    AddressDetail = "Số 12, ngõ 34",
                    Commune = "Mai Dịch",
                    District = "Cầu Giấy",
                    Province = "Hà Nội",
                    FullAddress = "Số 12, ngõ 34, Phường Mai Dịch, Quận Cầu Giấy, Hà Nội",
                    Latitude = 21.0367,
                    Longitude = 105.7825,
                    IsDefault = true,
                    Status = PublicStatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                };

                context.Addresses.Add(address);
                await context.SaveChangesAsync();
            }

            // --- 11. Payment Methods ---
            if (!await context.PaymentMethods.AnyAsync())
            {
                context.PaymentMethods.AddRange(
                    new PaymentMethod
                    {
                        Name = "Thanh toán khi nhận hàng (COD)",
                        PaymentType = PaymentTypeEnum.COD,
                        ImageUrl = "https://cdn-icons-png.flaticon.com/512/2331/2331941.png",
                        ProcessingFee = 0,
                        SortOrder = 1,
                        Status = PublicStatusEnum.Active,
                        CreatedAt = DateTime.UtcNow
                    },
                    new PaymentMethod
                    {
                        Name = "Chuyển khoản Ngân hàng (VietQR)",
                        PaymentType = PaymentTypeEnum.BankTransfer,
                        ImageUrl = "https://img.vietqr.io/image/MB-123456789-compact2.png", // Demo
                        // Cấu hình VietQR
                        BankName = "MB",
                        BankAccountNumber = "0393742967",
                        BankAccountName = "LÊ HUY HOÀN" ,
                        QRTplUrl = "compact2",
                        Instructions = "Vui lòng ghi nội dung chuyển khoản là Mã đơn hàng.",
                        ProcessingFee = 0,
                        SortOrder = 2,
                        Status = PublicStatusEnum.Active,
                        CreatedAt = DateTime.UtcNow
                    },
                    new PaymentMethod
                    {
                        Name = "Ví MoMo",
                        PaymentType = PaymentTypeEnum.EWallet,
                        ImageUrl = "https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png",
                        ProcessingFee = 0,
                        SortOrder = 3,
                        Status = PublicStatusEnum.Inactive, // Tạm tắt để demo tính năng Toggle
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("--> Đã tạo Payment Methods");
            }

            Console.WriteLine("✅ Database seeding completed successfully!");
        }
    }
}