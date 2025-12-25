using drinking_be.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class DBDrinkContext : DbContext
{
    public DBDrinkContext()
    {
    }

    public DBDrinkContext(DbContextOptions<DBDrinkContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<Attendance> Attendances { get; set; }
    public  virtual DbSet<Banner> Banners { get; set; }
    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<FranchiseRequest> FranchiseRequests { get; set; }
    public virtual DbSet<Inventory> Inventories { get; set; }
    public virtual DbSet<Membership> Memberships { get; set; }
    public virtual DbSet<MembershipLevel> MembershipLevels { get; set; }
    public virtual DbSet<News> News { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<OrderPayment> OrderPayments { get; set; }
    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
    public virtual DbSet<Policy> Policies { get; set; }
    public virtual DbSet<PointHistory> PointHistories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<ProductStore> ProductStores { get; set; }
    public virtual DbSet<Reservation> Reservations { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<ShopTable> ShopTables { get; set; }
    public virtual DbSet<Size> Sizes { get; set; }
    public virtual DbSet<SocialMedia> SocialMedias { get; set; }
    public virtual DbSet<Staff> Staffs { get; set; }
    public virtual DbSet<Store> Stores { get; set; }
    public virtual DbSet<SupplyOrder> SupplyOrders { get; set; }
    public virtual DbSet<SupplyOrderItem> SupplyOrderItems { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserVoucher> UserVouchers { get; set; }
    public virtual DbSet<VoucherTemplate> VoucherTemplates { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("address");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Address_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.RecipientName)
                  .HasMaxLength(200)
                  .HasColumnName("recipient_name");

            entity.Property(e => e.RecipientPhone)
                  .HasMaxLength(50)
                  .HasColumnName("recipient_phone");

            entity.Property(e => e.FullAddress)
                  .IsRequired()
                  .HasMaxLength(1000)
                  .HasColumnName("full_address");

            entity.Property(e => e.AddressDetail)
                  .IsRequired()
                  .HasMaxLength(500)
                  .HasColumnName("address_detail");

            entity.Property(e => e.Province)
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("province");

            entity.Property(e => e.District)
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("district");

            entity.Property(e => e.Commune)
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("commune");

            entity.Property(e => e.Latitude)
                  .HasColumnName("latitude");

            entity.Property(e => e.Longitude)
                  .HasColumnName("longitude");

            entity.Property(e => e.IsDefault)
                  .HasDefaultValue(false)
                  .HasColumnName("is_default");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- RELATIONSHIPS ---

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Addresses)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-1 với Store, FK nằm ở Store.AddressId
            //entity.HasOne(e => e.Store)
            //      .WithOne(s => s.Address)
            //      .HasForeignKey<Store>(s => s.AddressId)
            //      .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Attendance_Id");
            entity.ToTable("attendance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            // Cấu hình DateOnly
            entity.Property(e => e.Date)
                .HasColumnType("date")
                .HasColumnName("date");

            entity.Property(e => e.CheckInTime).HasColumnName("check_in_time");
            entity.Property(e => e.CheckOutTime).HasColumnName("check_out_time");

            entity.Property(e => e.WorkingHours).HasColumnName("working_hours");
            entity.Property(e => e.OvertimeHours).HasColumnName("overtime_hours");

            // Enum Status
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(AttendanceStatusEnum.Absent)
                .HasConversion<short>();

            entity.Property(e => e.Note).HasMaxLength(500).HasColumnName("note");

            // Tiền
            entity.Property(e => e.DailyBonus)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("daily_bonus");

            entity.Property(e => e.DailyDeduction)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("daily_deduction");

            // Audit
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(NOW())").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(NOW())").HasColumnName("updated_at");

            // Relationships
            entity.HasOne(d => d.Staff)
                .WithMany() // Staff không cần list Attendance (nếu cần thì thêm ICollection<Attendance> vào Staff)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa Attendance nếu xóa Staff (để giữ lịch sử)

            entity.HasOne(d => d.Store)
                .WithMany()
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("brand");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Brand_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasDefaultValueSql("gen_random_uuid()")
                  .HasColumnName("public_id");

            entity.HasIndex(e => e.PublicId)
                  .IsUnique();

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("name");

            entity.Property(e => e.LogoUrl)
                  .HasColumnName("logo_url");

            entity.Property(e => e.Address)
                  .HasColumnName("address");

            entity.Property(e => e.Hotline)
                  .HasMaxLength(50)
                  .HasColumnName("hotline");

            entity.Property(e => e.EmailSupport)
                  .HasMaxLength(200)
                  .HasColumnName("email_support");

            entity.Property(e => e.TaxCode)
                  .HasMaxLength(50)
                  .HasColumnName("tax_code");

            entity.Property(e => e.CompanyName)
                  .HasMaxLength(200)
                  .HasColumnName("company_name");

            entity.Property(e => e.Slogan)
                  .HasMaxLength(300)
                  .HasColumnName("slogan");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.CopyrightText)
                  .HasColumnName("copyright_text");

            entity.Property(e => e.EstablishedDate)
                  .HasColumnName("established_date");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.ToTable("banner");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.ImageUrl)
                  .IsRequired()
                  .HasMaxLength(500)
                  .HasColumnName("image_url");

            entity.Property(e => e.Title)
                  .HasMaxLength(200)
                  .HasColumnName("title");

            entity.Property(e => e.LinkUrl)
                  .HasMaxLength(500)
                  .HasColumnName("link_url");

            entity.Property(e => e.SortOrder)
                  .HasDefaultValue(0)
                  .HasColumnName("sort_order");

            entity.Property(e => e.Position)
                  .HasMaxLength(50)
                  .HasColumnName("position");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.StartAt)
                  .HasColumnName("start_at");

            entity.Property(e => e.EndAt)
                  .HasColumnName("end_at");

            entity.Property(e => e.IsClickable)
                  .HasDefaultValue(true)
                  .HasColumnName("is_clickable");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // Index phục vụ query nhanh banner đang hiển thị
            entity.HasIndex(e => new { e.Position, e.Status });
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("cart");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id")
                  .IsRequired();

            entity.Property(e => e.StoreId)
                  .HasColumnName("store_id")
                  .IsRequired();

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(CartStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            // --- RELATIONSHIPS ---
            entity.HasOne(e => e.User)
                  .WithOne(u => u.Cart)
                  .HasForeignKey<Cart>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Store)
                  .WithMany()
                  .HasForeignKey(e => e.StoreId)
                  .OnDelete(DeleteBehavior.Restrict);

            // --- LOGICAL CONSTRAINT ---
            entity.HasIndex(e => new { e.UserId, e.Status });
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("cart_item");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.CartId)
                  .HasColumnName("cart_id")
                  .IsRequired();

            entity.Property(e => e.ProductId)
                  .HasColumnName("product_id")
                  .IsRequired();

            entity.Property(e => e.SizeId)
                  .HasColumnName("size_id");

            entity.Property(e => e.Quantity)
                  .HasColumnName("quantity")
                  .IsRequired();

            entity.Property(e => e.BasePrice)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("base_price");

            entity.Property(e => e.FinalPrice)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("final_price");

            entity.Property(e => e.SugarLevel)
                  .HasConversion<short>()
                  .HasColumnName("sugar_level");

            entity.Property(e => e.IceLevel)
                  .HasConversion<short>()
                  .HasColumnName("ice_level");

            entity.Property(e => e.Note)
                  .HasMaxLength(500)
                  .HasColumnName("note");

            entity.Property(e => e.ParentItemId)
                  .HasColumnName("parent_item_id");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            // --- RELATIONSHIPS ---
            entity.HasOne(e => e.Cart)
                  .WithMany(c => c.CartItems)
                  .HasForeignKey(e => e.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.CartItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Size)
                  .WithMany(s => s.CartItems)
                  .HasForeignKey(e => e.SizeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ParentItem)
                  .WithMany(p => p.InverseParentItem)
                  .HasForeignKey(e => e.ParentItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("category");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Category_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.ParentId)
                  .HasColumnName("parent_id");

            entity.Property(e => e.Slug)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("slug");

            entity.HasIndex(e => e.Slug)
                  .IsUnique();

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("name");

            entity.Property(e => e.SortOrder)
                  .HasColumnName("sort_order");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- SELF-REFERENCE (CATEGORY TREE) ---
            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.InverseParent)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comment");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id")
                  .IsRequired();

            entity.Property(e => e.NewsId)
                  .HasColumnName("news_id")
                  .IsRequired();

            entity.Property(e => e.ParentId)
                  .HasColumnName("parent_id");

            entity.Property(e => e.Content)
                  .HasColumnName("content")
                  .HasMaxLength(2000)
                  .IsRequired();

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(ReviewStatusEnum.Pending)
                  .HasColumnName("status");

            entity.Property(e => e.IsEdited)
                  .HasDefaultValue(false)
                  .HasColumnName("is_edited");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- RELATIONSHIPS ---

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Comments)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.News)
                  .WithMany(n => n.Comments)
                  .HasForeignKey(e => e.NewsId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Parent)
                  .WithMany(p => p.InverseParent)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FranchiseRequest>(entity =>
        {
            entity.ToTable("franchise_request"); // Tên bảng lowercase

            entity.HasKey(e => e.Id).HasName("PK_FranchiseRequest_Id");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            // --- Thông tin ứng viên ---
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("full_name");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("email");

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false) // Số điện thoại không cần Unicode
                .HasColumnName("phone_number");

            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");

            // --- Thông tin dự án ---
            entity.Property(e => e.TargetArea)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("target_area");

            entity.Property(e => e.EstimatedBudget)
                .HasColumnType("decimal(18,2)") // Định dạng tiền tệ
                .HasColumnName("estimated_budget");

            entity.Property(e => e.ExperienceDescription)
                .HasColumnType("text") // Cho phép nhập dài
                .HasColumnName("experience_description");

            // --- Quản lý & Trạng thái ---
            // Lưu Enum dưới dạng String để dễ đọc trong DB (VD: 'Pending', 'Approved')
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(FranchiseStatusEnum.Pending)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.Property(e => e.AdminNote)
                .HasMaxLength(1000)
                .HasColumnName("admin_note");

            entity.Property(e => e.ReviewerId)
                .HasColumnName("reviewer_id");

            // --- Audit Timestamps ---
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            // --- Relationships ---
            entity.HasOne(d => d.Reviewer)
                .WithMany() // User không cần list ngược lại
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.SetNull) // Xóa nhân viên -> Set hồ sơ về null (không xóa hồ sơ)
                .HasConstraintName("FK_franchise_request_reviewer");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("inventory");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasColumnName("public_id");

            entity.Property(e => e.MaterialId)
                  .HasColumnName("material_id")
                  .IsRequired();

            entity.Property(e => e.StoreId)
                  .HasColumnName("store_id");

            entity.Property(e => e.Quantity)
                  .HasColumnName("quantity")
                  .IsRequired();

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.RowVersion)
                  .IsRowVersion()
                  .HasColumnName("row_version");

            // --- UNIQUE CONSTRAINT ---
            entity.HasIndex(e => new { e.MaterialId, e.StoreId })
                  .IsUnique();

            // --- RELATIONSHIPS ---
            entity.HasOne(e => e.Material)
                  .WithMany(m => m.Inventories)
                  .HasForeignKey(e => e.MaterialId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Store)
                  .WithMany()
                  .HasForeignKey(e => e.StoreId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("material");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasColumnName("public_id");

            entity.Property(e => e.Name)
                  .HasMaxLength(255)
                  .IsRequired()
                  .HasColumnName("name");

            entity.Property(e => e.Description)
                  .HasColumnName("description");

            entity.Property(e => e.ImageUrl)
                  .HasColumnName("image_url");

            entity.Property(e => e.BaseUnit)
                  .HasConversion<short>()
                  .HasColumnName("base_unit");

            entity.Property(e => e.PurchaseUnit)
                  .HasConversion<short>()
                  .HasColumnName("purchase_unit");

            entity.Property(e => e.ConversionRate)
                  .HasColumnName("conversion_rate");

            entity.Property(e => e.CostPerPurchaseUnit)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("cost_per_purchase_unit");

            entity.Property(e => e.MinStockAlert)
                  .HasColumnName("min_stock_alert");

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true)
                  .HasColumnName("is_active");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // Relationships
            entity.HasMany(e => e.Inventories)
                  .WithOne(i => i.Material)
                  .HasForeignKey(i => i.MaterialId);

            entity.HasMany(e => e.SupplyOrderItems)
                  .WithOne(s => s.Material)
                  .HasForeignKey(s => s.MaterialId);
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.ToTable("membership");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Membership_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            // --- FK ---
            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.MembershipLevelId)
                  .HasColumnName("membership_level_id");

            entity.HasIndex(e => e.UserId)
                  .IsUnique();

            // --- Card ---
            entity.Property(e => e.CardCode)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("card_code");

            entity.HasIndex(e => e.CardCode)
                  .IsUnique();

            // --- Loyalty ---
            entity.Property(e => e.CurrentCoins)
                  .HasDefaultValue(0)
                  .HasColumnName("current_coins");

            entity.Property(e => e.TotalSpent)
                  .HasColumnType("decimal(18,2)")
                  .HasDefaultValue(0)
                  .HasColumnName("total_spent");

            // --- Level lifecycle ---
            entity.Property(e => e.LevelStartDate)
                  .HasColumnName("level_start_date");

            entity.Property(e => e.LevelEndDate)
                  .HasColumnName("level_end_date");

            entity.Property(e => e.LastLevelSpentReset)
                  .HasColumnName("last_level_spent_reset");

            // --- Status ---
            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(MembershipStatusEnum.Active)
                  .HasColumnName("status");

            // --- Audit ---
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- Relationships ---
            entity.HasOne(e => e.User)
                  .WithOne(u => u.Membership)
                  .HasForeignKey<Membership>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Level)
                  .WithMany(l => l.Memberships)
                  .HasForeignKey(e => e.MembershipLevelId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MembershipLevel>(entity =>
        {
            entity.ToTable("membership_level");

            entity.HasKey(e => e.Id)
                  .HasName("PK_MembershipLevel_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("name");

            entity.Property(e => e.RankOrder)
                  .HasColumnName("rank_order");

            entity.HasIndex(e => e.RankOrder)
                  .IsUnique();

            entity.Property(e => e.MinCoinsRequired)
                  .HasDefaultValue(0)
                  .HasColumnName("min_coins_required");

            entity.Property(e => e.DurationDays)
                  .HasColumnName("duration_days");

            entity.Property(e => e.Benefits)
                  .HasMaxLength(1000)
                  .HasColumnName("benefits");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");
            
            entity.Property(e => e.PointEarningRate)
                  .HasDefaultValue(0);

            entity.Property(e => e.ResetReductionPercent)
                  .HasDefaultValue(0);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.ToTable("news");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasDefaultValueSql("gen_random_uuid()")
                  .HasColumnName("public_id");

            entity.HasIndex(e => e.PublicId).IsUnique();

            entity.Property(e => e.Slug)
                  .IsRequired()
                  .HasMaxLength(300)
                  .HasColumnName("slug");

            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Type)
                  .HasConversion<short>()
                  .HasColumnName("type");

            entity.Property(e => e.Title)
                  .IsRequired()
                  .HasMaxLength(300)
                  .HasColumnName("title");

            entity.Property(e => e.Content)
                  .IsRequired()
                  .HasColumnName("content");

            entity.Property(e => e.ThumbnailUrl)
                  .HasColumnName("thumbnail_url");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(ContentStatusEnum.Draft)
                  .HasColumnName("status");

            entity.Property(e => e.IsFeatured)
                  .HasDefaultValue(false)
                  .HasColumnName("is_featured");

            entity.Property(e => e.ViewCount)
                  .HasDefaultValue(0)
                  .HasColumnName("view_count");

            entity.Property(e => e.SeoDescription)
                  .HasMaxLength(500)
                  .HasColumnName("seo_description");

            entity.Property(e => e.PublishedDate)
                  .HasColumnName("published_date");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.News)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notification"); // Tên bảng lowercase

            entity.HasKey(e => e.Id).HasName("PK_Notification_Id");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(4000)
                .HasColumnName("content");

            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<short>(); // Lưu Enum dưới dạng số (tinyint/smallint)

            entity.Property(e => e.ReferenceId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("reference_id");

            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");

            entity.Property(e => e.ScheduledTime)
                .IsRequired(false) // Cho phép null
                .HasColumnName("scheduled_time")
                .HasColumnType("timestamp without time zone");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())") // PostgreSQL syntax
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            // --- Indexes (Tối ưu truy vấn) ---
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_notification_user_id");

            entity.HasIndex(e => new { e.UserId, e.IsRead }) // Index kép để lọc tin chưa đọc
                .HasDatabaseName("IX_notification_user_read");

            entity.HasIndex(e => e.CreatedAt) // Index để sắp xếp thời gian
                .HasDatabaseName("IX_notification_created_at");

            // --- Relationships ---
            entity.HasOne(d => d.User)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .IsRequired(false) // Cho phép UserId null (Thông báo hệ thống)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_notification_user");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order"); // Tên bảng

            // --- CẤU HÌNH TAY TỪNG CỘT (Snake Case) ---
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderCode).IsRequired().HasColumnName("order_code");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.ShipperId).HasColumnName("shipper_id");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.DeliveryAddressId).HasColumnName("delivery_address_id");

            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingFee).HasColumnName("shipping_fee").HasColumnType("decimal(18,2)");
            entity.Property(e => e.GrandTotal).HasColumnName("grand_total").HasColumnType("decimal(18,2)");
            entity.Property(e => e.CoinsEarned).HasColumnName("coins_earned");

            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.OrderType).HasColumnName("order_type"); // Mới
            entity.Property(e => e.PickupCode).HasColumnName("pickup_code"); // Mới

            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.DeliveryDate).HasColumnName("delivery_date");

            entity.Property(e => e.UserVoucherId).HasColumnName("user_voucher_id");
            entity.Property(e => e.VoucherCodeUsed).HasColumnName("voucher_code_used");
            entity.Property(e => e.StoreName).HasColumnName("store_name");
            entity.Property(e => e.UserNotes).HasColumnName("user_notes");

            // Cancel info
            entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
            entity.Property(e => e.CancelNote).HasColumnName("cancel_note");
            entity.Property(e => e.CancelledByUserId).HasColumnName("cancelled_by_user_id");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            // Index & Quan hệ giữ nguyên như cũ
            entity.HasIndex(e => e.OrderCode).IsUnique();
            entity.HasIndex(e => e.PickupCode);

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Store).WithMany(p => p.Orders).HasForeignKey(d => d.StoreId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Table).WithMany(p => p.Orders).HasForeignKey(d => d.TableId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(d => d.DeliveryAddress).WithMany().HasForeignKey(d => d.DeliveryAddressId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Shipper).WithMany().HasForeignKey(d => d.ShipperId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_it__3213E83F36FD5A45");

            entity.ToTable("order_item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("base_price");
            entity.Property(e => e.FinalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("final_price");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ParentItemId).HasColumnName("parent_item_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SizeId).HasColumnName("size_id");

            entity.Property(e => e.IceLevel)
                .HasColumnName("ice_level")
                .HasConversion<short>();
            entity.Property(e => e.SugarLevel)
                .HasColumnName("sugar_level")
                .HasConversion<short>();

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_ite__order__2CF2ADDF");

            entity.HasOne(d => d.ParentItem).WithMany(p => p.InverseParentItem)
                .HasForeignKey(d => d.ParentItemId)
                .HasConstraintName("FK__Order_ite__paren__2EDAF651");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_ite__produ__2DE6D218");

            entity.HasOne(d => d.Size).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("FK__Order_ite__size___2FCF1A8A");
        });

        modelBuilder.Entity<OrderPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_OrderPayment_Id");
            entity.ToTable("order_payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("transaction_code");
            entity.Property(e => e.PaymentSignature)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("payment_signature");

            // ⭐ Enum Payment Status
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(OrderPaymentStatusEnum.Pending)
                .HasConversion<short>();

            entity.Property(e => e.PaymentDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_date");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            // Quan hệ với Order (1 Order có nhiều OrderPayments)
            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderPayment_OrderId");

            // Quan hệ với PaymentMethod
            entity.HasOne(d => d.PaymentMethod)
                .WithMany() // OrderPayment không cần Navigation Property ngược
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_OrderPayment_PaymentMethodId");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            // Giữ lại hoặc cập nhật khóa chính
            entity.HasKey(e => e.Id).HasName("PK_PaymentMethod_Id");

            entity.ToTable("payment_method");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("image_url");

            // --- Cấu hình Enum PaymentType ---
            entity.Property(e => e.PaymentType)
                .HasColumnName("payment_type")
                .HasDefaultValue(PaymentTypeEnum.COD)
                .HasConversion<short>(); // Lưu dưới dạng byte (1, 2, 3...)

            // --- Cấu hình các trường Chuyển khoản ---
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasColumnName("bank_name");
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bank_account_number");
            entity.Property(e => e.BankAccountName)
                .HasMaxLength(100)
                .HasColumnName("bank_account_name");
            entity.Property(e => e.Instructions)
                .HasMaxLength(500)
                .HasColumnName("instructions");
            entity.Property(e => e.QRTplUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("qr_tpl_url");

            // --- Cấu hình Trạng thái (Soft Delete) ---
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasConversion<short>(); // Lưu Enum Status
                                        // ⭐ BỔ SUNG: Cấu hình CreatedAt
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            // ⭐ BỔ SUNG: Cấu hình UpdatedAt
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            entity.Property(e => e.SortOrder).HasColumnName("sort_order");

            entity.Property(e => e.ProcessingFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("processing_fee");

            // Quan hệ 1-N với Order (giả định Order có khóa ngoại PaymentMethodId)
            entity.HasMany(d => d.Orders)
                .WithOne(p => p.PaymentMethod)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict) // Ngăn xóa PaymentMethod nếu còn Orders
                .HasConstraintName("FK_Order_PaymentMethodId");
        });

        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.ToTable("payslip"); // Tên bảng lowercase

            entity.HasKey(e => e.Id).HasName("PK_Payslip_Id");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.StaffId)
                .HasColumnName("staff_id");

            // --- Kỳ lương ---
            entity.Property(e => e.Month)
                .HasColumnName("month");

            entity.Property(e => e.Year)
                .HasColumnName("year");

            // Đảm bảo 1 nhân viên chỉ có 1 phiếu lương trong 1 tháng
            entity.HasIndex(e => new { e.StaffId, e.Month, e.Year })
                .IsUnique()
                .HasDatabaseName("IX_payslip_staff_month_year");

            // --- Thời gian ---
            entity.Property(e => e.FromDate)
                .HasColumnType("date") // PostgreSQL date
                .HasColumnName("from_date");

            entity.Property(e => e.ToDate)
                .HasColumnType("date")
                .HasColumnName("to_date");

            // --- Snapshot Cấu hình lương ---
            entity.Property(e => e.AppliedSalaryType)
                .HasConversion<short>()
                .HasColumnName("applied_salary_type");

            entity.Property(e => e.AppliedBaseSalary)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("applied_base_salary");

            entity.Property(e => e.AppliedHourlyRate)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("applied_hourly_rate");

            entity.Property(e => e.AppliedOvertimeRate)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("applied_overtime_rate");

            // --- Tổng hợp công (Double/Int) ---
            entity.Property(e => e.TotalWorkHours)
                .HasDefaultValue(0)
                .HasColumnName("total_work_hours");

            entity.Property(e => e.TotalOvertimeHours)
                .HasDefaultValue(0)
                .HasColumnName("total_overtime_hours");

            entity.Property(e => e.TotalWorkDays)
                .HasDefaultValue(0)
                .HasColumnName("total_work_days");

            // --- Chi tiết Thu nhập (Decimal) ---
            entity.Property(e => e.SalaryBeforeTax)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("salary_before_tax");

            entity.Property(e => e.Allowance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("allowance");

            entity.Property(e => e.Bonus)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("bonus");

            entity.Property(e => e.Deduction)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("deduction");

            entity.Property(e => e.TaxAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("tax_amount");

            entity.Property(e => e.FinalSalary)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0)
                .HasColumnName("final_salary");

            // --- Quản trị ---
            entity.Property(e => e.Status)
                .HasConversion<short>()
                .HasDefaultValue(PayslipStatusEnum.Draft)
                .HasColumnName("status");

            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");

            // --- Audit Timestamps ---
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            // --- Relationships ---
            entity.HasOne(d => d.Staff)
                .WithMany() // Staff không cần list Payslips ngược lại (hoặc thêm vào Staff nếu cần)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Restrict) // Không xóa Payslip khi xóa Staff (giữ lịch sử kế toán)
                .HasConstraintName("FK_payslip_staff");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Policy__3213E83FB49E512D");

            entity.ToTable("policy");

            entity.HasIndex(e => e.Slug, "UQ__Policy__32DD1E4C980885E3").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Status)
                .HasDefaultValue(PolicyReviewStatusEnum.Pending)
                .HasColumnName("status")
                .HasConversion<short>();
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Brand).WithMany(p => p.Policies)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Policy__brand_id__5BE2A6F2");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            // Thiết lập khóa ngoại tới Store
            entity.HasOne(d => d.Store)
                .WithMany(p => p.Policies) // Cần thêm ICollection<Policy> Policies vào Store.cs
                .HasForeignKey(d => d.StoreId)
                .HasConstraintName("FK_Policy_StoreId");
        });

        modelBuilder.Entity<PointHistory>(entity =>
        {
            entity.ToTable("point_history");

            // 1. Khóa chính
            entity.HasKey(e => e.Id)
                  .HasName("PK_PointHistory_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            // 2. Các cột cơ bản
            entity.Property(e => e.MembershipId)
                  .IsRequired()
                  .HasColumnName("membership_id");

            entity.Property(e => e.OrderId)
                  .HasColumnName("order_id");

            entity.Property(e => e.Amount)
                  .IsRequired()
                  .HasColumnName("amount");

            entity.Property(e => e.BalanceAfter)
                  .IsRequired()
                  .HasColumnName("balance_after");

            // 3. Enum/string mapping
            entity.Property(e => e.TransactionType)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("transaction_type")
                  .HasConversion<string>(); // Lưu dưới dạng string

            entity.Property(e => e.Description)
                  .IsRequired()
                  .HasMaxLength(500)
                  .HasColumnName("description");

            // 4. Thời gian
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            // 5. Indexes (quan trọng cho query)
            entity.HasIndex(e => e.MembershipId)
                  .HasDatabaseName("IX_point_history_membership_id");

            entity.HasIndex(e => e.OrderId)
                  .HasDatabaseName("IX_point_history_order_id");

            entity.HasIndex(e => e.CreatedAt)
                  .HasDatabaseName("IX_point_history_created_at");

            entity.HasIndex(e => e.TransactionType)
                  .HasDatabaseName("IX_point_history_transaction_type");

            // 6. Quan hệ
            entity.HasOne(e => e.Membership)
                  .WithMany(m => m.PointHistories) // Cần thêm ICollection<PointHistory> vào Membership
                  .HasForeignKey(e => e.MembershipId)
                  .OnDelete(DeleteBehavior.Restrict); // Không xóa khi Membership bị xóa (giữ lịch sử)

            entity.HasOne(e => e.Order)
                  .WithMany() // Order không cần navigation property ngược
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.SetNull); // Nếu xóa Order, set OrderId = null
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Product_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasDefaultValueSql("gen_random_uuid()")
                  .HasColumnName("public_id");

            entity.HasIndex(e => e.PublicId)
                  .IsUnique();

            entity.Property(e => e.Slug)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("slug");

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("name");

            entity.Property(e => e.ProductType)
                  .HasConversion<short>()
                  .HasColumnName("product_type");

            entity.Property(e => e.BasePrice)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("base_price");

            entity.Property(e => e.Description)
                  .HasColumnName("description");

            entity.Property(e => e.ImageUrl)
                  .HasColumnName("image_url");

            entity.Property(e => e.Ingredient)
                  .HasColumnName("ingredient");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(ProductStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.TotalRating)
                  .HasColumnName("total_rating");

            entity.Property(e => e.TotalSold)
                  .HasColumnName("total_sold");

            entity.Property(e => e.SearchVector)
                  .HasColumnName("search_vector");

            entity.Property(e => e.LaunchDateTime)
                  .HasColumnName("launch_datetime");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- RELATIONSHIP ---
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId);
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.ToTable("product_size");

            // --- COMPOSITE KEY ---
            entity.HasKey(e => new { e.ProductId, e.SizeId })
                  .HasName("PK_ProductSize");

            entity.Property(e => e.ProductId)
                  .HasColumnName("product_id");

            entity.Property(e => e.SizeId)
                  .HasColumnName("size_id");

            entity.Property(e => e.PriceOverride)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("price_override");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.SortOrder)
                  .HasColumnName("sort_order");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- RELATIONSHIPS ---
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.ProductSizes)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Size)
                  .WithMany(s => s.ProductSizes)
                  .HasForeignKey(e => e.SizeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductStore>(entity =>
        {
            entity.ToTable("product_store");

            // --- COMPOSITE KEY ---
            entity.HasKey(e => new { e.ProductId, e.StoreId })
                  .HasName("PK_ProductStore");

            entity.Property(e => e.ProductId)
                  .HasColumnName("product_id");

            entity.Property(e => e.StoreId)
                  .HasColumnName("store_id");

            entity.Property(e => e.SoldCount)
                  .HasDefaultValue(0)
                  .HasColumnName("sold_count");
            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(ProductStoreStatusEnum.Available)
                  .HasColumnName("status");

            entity.Property(e => e.PriceOverride)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("price_override");

            entity.Property(e => e.SortOrder)
                  .HasColumnName("sort_order");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- RELATIONSHIPS ---
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.ProductStores)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Store)
                  .WithMany(s => s.ProductStores)
                  .HasForeignKey(e => e.StoreId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_reservation");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.ReservationCode, "UQ__Reservat__FA8FADE431DEB25F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedTableId).HasColumnName("assigned_table_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .HasColumnName("customer_name");
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("customer_phone");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.NumberOfGuests).HasColumnName("number_of_guests");
            entity.Property(e => e.ReservationCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("reservation_code");
            entity.Property(e => e.ReservationDatetime)
                .HasColumnName("reservation_datetime");
            // ⭐ BỔ SUNG: Cấu hình DepositAmount
            entity.Property(e => e.DepositAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)") // Cấu hình kiểu thập phân
                .HasColumnName("deposit_amount");

            // ⭐ BỔ SUNG: Cấu hình IsDepositPaid
            entity.Property(e => e.IsDepositPaid)
                .HasDefaultValue(false)
                .HasColumnName("is_deposit_paid");
            entity.Property(e => e.Status)
                .HasDefaultValue(ReservationStatusEnum.Pending)
                .HasColumnName("status")
                .HasConversion<short>();
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AssignedTable).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.AssignedTableId)
                .HasConstraintName("FK__Reservati__assig__17C286CF");

            entity.HasOne(d => d.Store).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__store__16CE6296");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reservati__user___15DA3E5D");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("review");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.ProductId)
                  .HasColumnName("product_id")
                  .IsRequired();

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id")
                  .IsRequired();

            entity.Property(e => e.Content)
                  .HasColumnName("content")
                  .HasMaxLength(2000);

            entity.Property(e => e.Rating)
                  .HasColumnName("rating")
                  .IsRequired();

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(ReviewStatusEnum.Pending)
                  .HasColumnName("status");

            entity.Property(e => e.MediaUrl)
                  .HasColumnName("media_url");

            entity.Property(e => e.AdminResponse)
                  .HasColumnName("admin_response")
                  .HasMaxLength(1000);

            entity.Property(e => e.IsEdited)
                  .HasDefaultValue(false)
                  .HasColumnName("is_edited");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- UNIQUE CONSTRAINT ---
            entity.HasIndex(e => new { e.UserId, e.ProductId })
                  .IsUnique();

            // --- RELATIONSHIPS ---
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Reviews)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Reviews)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("room"); // Tên bảng lowercase

            entity.HasKey(e => e.Id).HasName("pk_room_td");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.StoreId)
                .HasColumnName("store_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100) // Khớp với DTO
                .HasColumnName("name");

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");

            entity.Property(e => e.Capacity)
                .HasDefaultValue(0)
                .HasColumnName("capacity");

            // --- Đặc điểm phòng ---
            entity.Property(e => e.IsAirConditioned)
                .HasDefaultValue(true) // Mặc định có máy lạnh
                .HasColumnName("is_air_conditioned");

            entity.Property(e => e.IsSmokingAllowed)
                .HasDefaultValue(false) // Mặc định cấm thuốc
                .HasColumnName("is_smoking_allowed");

            // --- Trạng thái ---
            entity.Property(e => e.Status)
                .HasConversion<short>()
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasColumnName("status");

            // --- Audit Timestamps ---
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            // --- Relationships ---
            entity.HasOne(d => d.Store)
                .WithMany(p => p.Rooms)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa Store -> Xóa luôn các phòng
                .HasConstraintName("fk_room_store");
        });

        modelBuilder.Entity<ShopTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Table__3213E83FCBCE513F");

            entity.ToTable("shop_Table");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CanBeMerged)
                .HasDefaultValue(true)
                .HasColumnName("can_be_merged");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Status)
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasColumnName("status")
                .HasConversion<short>();
            entity.Property(e => e.MergedWithTableId).HasColumnName("merged_with_table_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            entity.HasOne(d => d.MergedWithTable).WithMany(p => p.InverseMergedWithTable)
                .HasForeignKey(d => d.MergedWithTableId)
                .HasConstraintName("FK__Table__merged_wi__0D44F85C");

            entity.HasOne(d => d.Store).WithMany(p => p.ShopTables)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Table__store_id__0C50D423");
            entity.HasOne(d => d.Room)
                .WithMany(p => p.ShopTables)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.ToTable("size");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Size_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.Label)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasColumnName("label");

            entity.Property(e => e.PriceModifier)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("price_modifier");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<SocialMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SocialMedia_Id");

            entity.ToTable("social_media");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            // ⭐ BỔ SUNG: Cấu hình CreatedAt
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            // ⭐ BỔ SUNG: Cấu hình UpdatedAt
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)   
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
                
            entity.Property(e => e.Platform)
                .HasConversion<short>()
                .HasColumnName("platform_name");

            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("url");

            entity.Property(e => e.IconUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("icon_url");

            entity.Property(e => e.SortOrder).HasColumnName("sort_order");

            // ⭐ Áp dụng Enum và HasConversion
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasConversion<short>();

            // 1. Quan hệ với Brand (BẮT BUỘC)
            entity.HasOne(d => d.Brand)
                .WithMany(p => p.SocialMedias) // Cần thêm ICollection<SocialMedia> SocialMedias vào Brand.cs
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocialMedia_BrandId");

            // 2. Quan hệ với Store (TÙY CHỌN)
            entity.HasOne(d => d.Store)
                .WithMany(p => p.SocialMedias)
                .HasForeignKey(d => d.StoreId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SocialMedia_StoreId");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Staff_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasDefaultValueSql("gen_random_uuid()")
                  .HasColumnName("public_id");

            entity.HasIndex(e => e.PublicId)
                  .IsUnique();

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.HasIndex(e => e.UserId)
                  .IsUnique();

            entity.Property(e => e.StoreId)
                  .HasColumnName("store_id");

            entity.Property(e => e.FullName)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("full_name");

            entity.Property(e => e.CitizenId)
                  .HasMaxLength(50)
                  .HasColumnName("citizen_id");

            entity.Property(e => e.DateOfBirth)
                  .HasColumnName("date_of_birth");

            entity.Property(e => e.Address)
                  .HasMaxLength(500)
                  .HasColumnName("address");

            entity.Property(e => e.Position)
                  .HasConversion<short>()
                  .HasDefaultValue(StaffPositionEnum.Server)
                  .HasColumnName("position");

            entity.Property(e => e.SalaryType)
                  .HasConversion<short>()
                  .HasDefaultValue(SalaryTypeEnum.PartTime)
                  .HasColumnName("salary_type");

            entity.Property(e => e.BaseSalary)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("base_salary");

            entity.Property(e => e.HourlySalary)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("hourly_salary");

            entity.Property(e => e.OvertimeHourlySalary)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("overtime_hourly_salary");

            entity.Property(e => e.MinWorkHoursPerMonth)
                  .HasColumnName("min_work_hours_per_month");

            entity.Property(e => e.MaxWorkHoursPerMonth)
                  .HasColumnName("max_work_hours_per_month");

            entity.Property(e => e.MaxOvertimeHoursPerMonth)
                  .HasColumnName("max_overtime_hours_per_month");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.HireDate)
                  .HasColumnName("hire_date");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            entity.HasOne(e => e.User)
                  .WithOne(u => u.Staff)
                  .HasForeignKey<Staff>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Store)
                  .WithMany(s => s.Staffs)
                  .HasForeignKey(e => e.StoreId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("store");

            entity.HasKey(e => e.Id)
                  .HasName("PK_Store_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasDefaultValueSql("gen_random_uuid()")
                  .HasColumnName("public_id");

            entity.HasIndex(e => e.PublicId)
                  .IsUnique();

            entity.Property(e => e.Slug)
                  .HasMaxLength(200)
                  .HasColumnName("slug");

            entity.HasIndex(e => e.Slug)
                  .IsUnique()
                  .HasFilter("slug IS NOT NULL");

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("name");

            entity.Property(e => e.ImageUrl)
                  .HasColumnName("image_url");

            entity.Property(e => e.OpenDate)
                  .HasColumnName("open_date");

            entity.Property(e => e.OpenTime)
                  .HasColumnName("open_time");

            entity.Property(e => e.CloseTime)
                  .HasColumnName("close_time");

            entity.Property(e => e.ShippingFeeFixed)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("shipping_fee_fixed");

            entity.Property(e => e.ShippingFeePerKm)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("shipping_fee_per_km");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(StoreStatusEnum.ComingSoon)
                  .HasColumnName("status");

            entity.Property(e => e.SortOrder)
                  .HasColumnName("sort_order");

            entity.Property(e => e.MapVerified)
                  .HasDefaultValue(false)
                  .HasColumnName("map_verified");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            entity.Property(e => e.AddressId)
                  .HasColumnName("address_id");

            entity.HasOne(s => s.Address)
                  .WithOne(a => a.Store)  // Address.Store có thể null
                  .HasForeignKey<Store>(s => s.AddressId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();

            entity.HasOne(e => e.Brand)
                  .WithMany(b => b.Stores)
                  .HasForeignKey(e => e.BrandId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SupplyOrder>(entity =>
        {
            entity.ToTable("supply_order");
            entity.HasKey(e => e.Id).HasName("PK_SupplyOrder_Id");

            entity.HasIndex(e => e.PublicId).IsUnique();
            entity.HasIndex(e => e.OrderCode).IsUnique();

            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("public_id");

            entity.Property(e => e.OrderCode).HasMaxLength(20).IsRequired().HasColumnName("order_code");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnName("total_amount");

            entity.Property(e => e.Note).HasMaxLength(500).HasColumnName("note");

            entity.Property(e => e.Status)
                .HasDefaultValue(SupplyOrderStatusEnum.Pending)
                .HasColumnName("status")
                .HasConversion<short>();

            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("approved_by_user_id");

            // Audit
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(NOW())").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(NOW())").HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at");
            entity.Property(e => e.ExpectedDeliveryDate).HasColumnName("expected_delivery_date");

            // Relationships
            entity.HasOne(d => d.Store)
                .WithMany() // Có thể thêm ICollection<SupplyOrder> vào Store nếu cần
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.SetNull); // Nếu Store xóa, giữ lại phiếu nhưng StoreId = null

            entity.HasOne(d => d.CreatedBy)
                .WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ApprovedBy)
                .WithMany()
                .HasForeignKey(d => d.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SupplyOrderItem>(entity =>
        {
            entity.ToTable("supply_order_item");
            entity.HasKey(e => e.Id).HasName("PK_SupplyOrderItem_Id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SupplyOrderId).HasColumnName("supply_order_id");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");

            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Unit).HasMaxLength(50).IsRequired().HasColumnName("unit");

            // Tiền
            entity.Property(e => e.CostPerUnit).HasColumnType("decimal(18, 2)").HasColumnName("cost_per_unit");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18, 2)").HasColumnName("total_cost");

            // Relationships
            entity.HasOne(d => d.SupplyOrder)
                .WithMany(p => p.SupplyOrderItems) // Cần thêm vào SupplyOrder.cs
                .HasForeignKey(d => d.SupplyOrderId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa phiếu -> Xóa chi tiết

            entity.HasOne(d => d.Material)
                .WithMany(p => p.SupplyOrderItems) // Đã thêm vào Material.cs ở bước trước
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.Restrict); // Không được xóa Material nếu đã có phiếu nhập
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasKey(e => e.Id)
                  .HasName("PK_User_Id");

            // --- Keys ---
            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.PublicId)
                  .HasColumnName("public_id")
                  .HasDefaultValueSql("gen_random_uuid()");

            entity.HasIndex(e => e.PublicId)
                  .IsUnique();

            // --- Identity ---
            entity.Property(e => e.Username)
                  .IsRequired()
                  .HasMaxLength(100)
                  .HasColumnName("username");

            entity.HasIndex(e => e.Username)
                  .IsUnique();

            entity.Property(e => e.Email)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("email");

            entity.HasIndex(e => e.Email)
                  .IsUnique();

            entity.Property(e => e.Phone)
                  .HasMaxLength(20)
                  .HasColumnName("phone");

            entity.Property(e => e.ThumbnailUrl)
                  .HasMaxLength(500)
                  .HasColumnName("thumbnail_url");

            entity.Property(e => e.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(500)
                  .HasColumnName("password_hash");

            // --- Enum ---
            entity.Property(e => e.RoleId)
                  .HasColumnName("role_id")
                  .HasConversion<short>()
                  .HasDefaultValue(UserRoleEnum.Customer);

            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasConversion<short>()
                  .HasDefaultValue(UserStatusEnum.Active);

            // --- Auth ---
            entity.Property(e => e.EmailVerified)
                  .HasDefaultValue(false)
                  .HasColumnName("email_verified");

            entity.Property(e => e.RefreshToken)
                  .HasMaxLength(500)
                  .HasColumnName("refresh_token");

            entity.Property(e => e.RefreshTokenExpiryTime)
                  .HasColumnName("refresh_token_expiry");

            entity.Property(e => e.ResetPasswordToken)
                  .HasMaxLength(500)
                  .HasColumnName("reset_password_token");

            entity.Property(e => e.ResetPasswordTokenExpiryTime)
                  .HasColumnName("reset_password_token_expiry");

            // --- Audit ---
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            entity.Property(e => e.LastLogin)
                  .HasColumnName("last_login");

            // --- Relationships ---
            entity.HasOne(e => e.Membership)
                  .WithOne(m => m.User)
                  .HasForeignKey<Membership>(m => m.UserId);

            entity.HasOne(e => e.Staff)
                  .WithOne(s => s.User)
                  .HasForeignKey<Staff>(s => s.UserId);
        });

        modelBuilder.Entity<UserVoucher>(entity =>
        {
            entity.ToTable("user_voucher");

            entity.HasKey(e => e.Id)
                  .HasName("PK_UserVoucher_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.UserId)
                  .HasColumnName("user_id");

            entity.Property(e => e.VoucherTemplateId)
                  .HasColumnName("voucher_template_id");

            entity.Property(e => e.VoucherCode)
                  .HasMaxLength(50)
                  .HasColumnName("voucher_code");

            entity.HasIndex(e => e.VoucherCode)
                  .IsUnique()
                  .HasFilter("voucher_code IS NOT NULL");

            entity.Property(e => e.IssuedDate)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("issued_date");

            entity.Property(e => e.ExpiryDate)
                  .HasColumnName("expiry_date");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(UserVoucherStatusEnum.Unused)
                  .HasColumnName("status");

            entity.Property(e => e.UsedDate)
                  .HasColumnName("used_date");

            entity.Property(e => e.OrderIdUsed)
                  .HasColumnName("order_id_used");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");

            // --- Relationships ---
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserVouchers)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.VoucherTemplate)
                  .WithMany(v => v.UserVouchers)
                  .HasForeignKey(e => e.VoucherTemplateId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OrderUsed)
                  .WithMany()
                  .HasForeignKey(e => e.OrderIdUsed)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<VoucherTemplate>(entity =>
        {
            entity.ToTable("voucher_template");

            entity.HasKey(e => e.Id)
                  .HasName("PK_VoucherTemplate_Id");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnName("name");

            entity.Property(e => e.MembershipLevelId)
                  .HasColumnName("membership_level_id");

            entity.HasOne(e => e.MembershipLevel)
                  .WithMany(l => l.VoucherTemplates)
                  .HasForeignKey(e => e.MembershipLevelId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.Property(e => e.DiscountType)
                  .HasConversion<short>()
                  .HasColumnName("discount_type");

            entity.Property(e => e.DiscountValue)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("discount_value");

            entity.Property(e => e.MinOrderValue)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("min_order_value");

            entity.Property(e => e.MaxDiscountAmount)
                  .HasColumnType("decimal(18,2)")
                  .HasColumnName("max_discount_amount");

            entity.Property(e => e.TotalQuantity)
                  .HasColumnName("total_quantity");

            entity.Property(e => e.UsedCount)
                  .HasDefaultValue(0)
                  .HasColumnName("used_count");

            entity.Property(e => e.UsageLimitPerUser)
                  .HasColumnName("usage_limit_per_user");

            entity.Property(e => e.CouponCode)
                  .HasMaxLength(50)
                  .HasColumnName("coupon_code");

            entity.HasIndex(e => e.CouponCode)
                  .IsUnique()
                  .HasFilter("coupon_code IS NOT NULL");

            entity.Property(e => e.StartDate)
                  .HasColumnName("start_date");

            entity.Property(e => e.EndDate)
                  .HasColumnName("end_date");

            entity.Property(e => e.Status)
                  .HasConversion<short>()
                  .HasDefaultValue(PublicStatusEnum.Active)
                  .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                  .HasColumnName("deleted_at");
        });



        OnModelCreatingPartial(modelBuilder);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName.ToLower());
            }
        }

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp without time zone");
                }
            }
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
