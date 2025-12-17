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
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductSize> ProductSizes { get; set; }
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
            entity.HasKey(e => e.Id).HasName("PK_Address_Id"); 

            entity.ToTable("address");

            // Bỏ UserId vì đã có trong cấu trúc Address trước đó
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            // Các trường mới: Thông tin người nhận
            entity.Property(e => e.RecipientName)
                .HasMaxLength(100)
                .HasColumnName("recipient_name");

            entity.Property(e => e.RecipientPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("recipient_phone");

            // Các trường địa chỉ chi tiết
            entity.Property(e => e.FullAddress)
                .HasMaxLength(500)
                .HasColumnName("full_address");

            entity.Property(e => e.AddressDetail)
                .HasMaxLength(255) // Giả định độ dài chi tiết
                .HasColumnName("address_detail");

            // Phân cấp hành chính
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .HasColumnName("province");

            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");

            entity.Property(e => e.Commune)
                .HasMaxLength(100)
                .HasColumnName("commune");

            // Tọa độ GPS (Bắt buộc)
            entity.Property(e => e.Latitude)
                .HasColumnType("double precision") // Dùng double precision cho tọa độ
                .HasColumnName("latitude");

            entity.Property(e => e.Longitude)
                .HasColumnType("double precision")
                .HasColumnName("longitude");

            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .HasColumnName("is_default");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(PublicStatusEnum.Active) // Set giá trị mặc định là 1 (Active)
                .HasConversion<byte>();

            // Thiết lập khóa ngoại tới User (một User có nhiều Address)
            entity.HasOne(d => d.User)
                .WithMany(p => p.Addresses) // Giả định User có Navigation Property là Addresses
                .HasForeignKey(d => d.UserId)
                .IsRequired(false) // Cho phép địa chỉ Store không có UserId
                .HasConstraintName("FK_Address_UserId");

            entity.HasOne(d => d.Store)
                .WithOne(p => p.Address) // Giả định Store có Navigation Property ngược là Address
                .HasForeignKey<Store>(d => d.AddressId) // Khóa ngoại StoreId sẽ được tham chiếu ngược từ Store
                .IsRequired(false) // Cho phép Address không thuộc Store
                .HasConstraintName("FK_Address_StoreId");
            entity.HasMany(d => d.Orders)
                .WithOne(p => p.DeliveryAddress) // Giả định Order có DeliveryAddress
                .HasForeignKey(d => d.DeliveryAddressId) // Khóa ngoại Order.DeliveryAddressId
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Order_DeliveryAddressId");
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
                .HasConversion<byte>();

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
            entity.HasKey(e => e.Id).HasName("PK__Brand__3213E83F3D8FA9ED");

            entity.ToTable("brand");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasColumnName("company_name");
            entity.Property(e => e.CopyrightText)
                .HasMaxLength(255)
                .HasColumnName("copyright_text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmailSupport)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email_support");
            entity.Property(e => e.EstablishedDate)
                .HasColumnType("date")
                .HasColumnName("established_date");
            entity.Property(e => e.Hotline)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("hotline");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("logo_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Slogan)
                .HasMaxLength(255)
                .HasColumnName("slogan");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(PublicStatusEnum.Active) // Set giá trị mặc định là 1 (Active)
                .HasConversion<byte>();
            entity.Property(e => e.TaxCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("tax_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Banner_Id");

            entity.ToTable("banner");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("image_url");

            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.Property(e => e.LinkUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("link_url");

            entity.Property(e => e.SortOrder)
                .HasDefaultValue(0)
                .HasColumnName("sort_order");

            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("position");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasConversion<byte>();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            // Indexes
            entity.HasIndex(e => new { e.Position, e.Status, e.SortOrder })
                .HasDatabaseName("IX_banner_position_status_order");

            entity.HasIndex(e => e.SortOrder)
                .HasDatabaseName("IX_banner_sort_order");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3213E83FED0FE127");

            entity.ToTable("Cart");

            entity.HasIndex(e => e.UserId, "UQ__Cart__B9BE370E2DCD722C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .HasConstraintName("FK__Cart__user_id__37703C52");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart_ite__3213E83F486E79A8");

            entity.ToTable("cart_item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("base_price");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.FinalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("final_price");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.ParentItemId).HasColumnName("parent_item_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SizeId).HasColumnName("size_id");
            
            entity.Property(e => e.IceLevel)
                .HasColumnName("ice_level")
                .HasConversion<byte>();
            entity.Property(e => e.SugarLevel)
                .HasColumnName("sugar_level")
                .HasConversion<byte>();

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart_item__cart___3B40CD36");

            entity.HasOne(d => d.ParentItem).WithMany(p => p.InverseParentItem)
                .HasForeignKey(d => d.ParentItemId)
                .HasConstraintName("FK__Cart_item__paren__3D2915A8");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart_item__produ__3C34F16F");

            entity.HasOne(d => d.Size).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("FK__Cart_item__size___3E1D39E1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3213E83F5A39BDEA");

            entity.ToTable("category");

            entity.HasIndex(e => e.Slug, "UQ__Category__32DD1E4C00F59B8C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.SortOrder)
                .HasDefaultValue((byte)0)
                .HasColumnName("sort_order");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(PublicStatusEnum.Active) // Set giá trị mặc định là 1 (Active)
                .HasConversion<byte>();
            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Category__parent__3C69FB99");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3213E83FE8B13CD2");

            entity.ToTable("comment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.NewsId).HasColumnName("news_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(ReviewStatusEnum.Pending)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.HasOne(d => d.News).WithMany(p => p.Comments)
                .HasForeignKey(d => d.NewsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__news_id__06CD04F7");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Comment__parent___04E4BC85");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__user_id__05D8E0BE");
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
            // 1. Tên bảng & Khóa chính
            entity.ToTable("inventory");
            entity.HasKey(e => e.Id).HasName("PK_Inventory_Id");

            // 2. Mapping tên cột (Quan trọng để đồng bộ)
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            entity.Property(e => e.Quantity)
                .HasDefaultValue(0) // Mặc định tồn kho là 0
                .HasColumnName("quantity");

            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(NOW())") // Tự động lấy giờ hiện tại
                .HasColumnName("last_updated");

            // 3. Index Unique Phức hợp
            // Đảm bảo 1 Material chỉ xuất hiện 1 lần tại 1 Store
            entity.HasIndex(e => new { e.MaterialId, e.StoreId }, "UQ_Inventory_Material_Store")
                  .IsUnique();

            // 4. Relationships
            // Quan hệ với Material
            entity.HasOne(d => d.Material)
                .WithMany(p => p.Inventories) // Đã có ICollection<Inventory> bên Material
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.Restrict); // Xóa Material không được nếu còn tồn kho

            // Quan hệ với Store
            entity.HasOne(d => d.Store)
                .WithMany() // Store không cần list Inventory (hoặc thêm nếu muốn)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Store -> Xóa luôn dòng tồn kho của Store đó
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("material"); // Tên bảng trong DB
            entity.HasKey(e => e.Id).HasName("PK_Material_Id");

            // --- ID & PublicId ---
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasIndex(e => e.PublicId).IsUnique();
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("gen_random_uuid()") // Tự động sinh GUID
                .HasColumnName("public_id");

            // --- Thông tin cơ bản ---
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("name");

            entity.Property(e => e.Description)
                .HasMaxLength(1000) // Cho phép mô tả dài hơn chút
                .HasColumnName("description");

            entity.Property(e => e.ImageUrl)
                .HasColumnName("image_url");

            // --- Đơn vị & Quy đổi ---
            entity.Property(e => e.BaseUnit)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("base_unit");

            entity.Property(e => e.PurchaseUnit)
                .HasMaxLength(50)
                .HasColumnName("purchase_unit");

            entity.Property(e => e.ConversionRate)
                .HasDefaultValue(1) // Mặc định 1:1 nếu không nhập
                .HasColumnName("conversion_rate");

            // --- Giá vốn ---
            entity.Property(e => e.CostPerPurchaseUnit)
                .HasColumnType("decimal(18, 2)") // Quan trọng cho tiền tệ
                .HasDefaultValue(0m)
                .HasColumnName("cost_per_purchase_unit");

            // ⭐ Bỏ qua thuộc tính tính toán (Computed Property) trong Database
            // Dù đã có [NotMapped] nhưng khai báo rõ ràng ở đây càng tốt
            entity.Ignore(e => e.CostPerBaseUnit);

            // --- Quản trị ---
            entity.Property(e => e.MinStockAlert)
                .HasColumnName("min_stock_alert");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");

            // --- Audit ---
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnName("updated_at");

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at");

            // --- Relationships (Sẽ cấu hình chi tiết khi tạo Inventory và SupplyOrder) ---
            // Hiện tại để mặc định EF Core tự hiểu hoặc cấu hình sau
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Membersh__3213E83FF483D767");

            entity.ToTable("membership");

            entity.HasIndex(e => e.CardCode, "UQ__Membersh__81703D727EAE3828").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__Membersh__B9BE370EFC41B82F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("card_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.LastLevelSpentReset)
                .HasColumnType("date")
                .HasColumnName("last_level_spent_reset");
            entity.Property(e => e.LevelEndDate)
                .HasColumnType("date")
                .HasColumnName("level_end_date");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.LevelStartDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnType("date")
                .HasColumnName("level_start_date");
            entity.Property(e => e.Status)
                .HasDefaultValue(MembershipStatusEnum.Active)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.TotalSpent)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_spent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Level).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Membershi__level__51300E55");

            entity.HasOne(d => d.User).WithOne(p => p.Membership)
                .HasForeignKey<Membership>(d => d.UserId)
                .HasConstraintName("FK__Membershi__user___503BEA1C");
        });

        modelBuilder.Entity<MembershipLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Membersh__3213E83F3901FC68");

            entity.ToTable("membership_Level");

            entity.HasIndex(e => e.Name, "UQ__Membersh__72E12F1BBCF20D34").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Benefits).HasColumnName("benefits");
            entity.Property(e => e.Status)
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.MinSpendRequired)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("min_spend_required");
            entity.Property(e => e.Name)
                .HasMaxLength(35)
                .HasColumnName("name");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3213E83F6A9F9780");

            entity.HasIndex(e => e.Slug, "UQ__News__32DD1E4C5A174899").IsUnique();

            entity.HasIndex(e => e.PublicId, "UQ__News__5699A53062598161").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsFeatured)
                .HasDefaultValue(false)
                .HasColumnName("is_featured");
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("public_id");
            entity.Property(e => e.PublishedDate)
                .HasColumnName("published_date");
            entity.Property(e => e.SeoDescription)
                .HasMaxLength(255)
                .HasColumnName("seo_description");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasDefaultValue(ContentStatusEnum.Draft)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("thumbnail_url");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__News__user_id__73BA3083");
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
                .HasConversion<byte>(); // Lưu Enum dưới dạng số (tinyint/smallint)

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
            entity.HasKey(e => e.Id).HasName("PK__Order__3213E83FDE5887F1");

            entity.ToTable("order");

            entity.HasIndex(e => e.OrderCode, "UQ__Order__99D12D3FD8ECB07F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CoinsEarned)
                .HasDefaultValue(0)
                .HasColumnName("coins_earned");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DeliveryDate)
                .HasColumnName("delivery_date");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.GrandTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("grand_total");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("order_code");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.ShippingFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("shipping_fee");
            entity.Property(e => e.Status)
                .HasDefaultValue(OrderStatusEnum.New)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.StoreName)
                .HasMaxLength(200)
                .HasColumnName("store_name");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserNotes)
                .HasMaxLength(500)
                .HasColumnName("user_notes");
            entity.Property(e => e.VoucherCodeUsed)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("voucher_code_used");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Order__payment_m__29221CFB");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__store_id__282DF8C2");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order__user_id__2739D489");

            entity.Property(e => e.DeliveryAddressId).HasColumnName("delivery_address_id");

            entity.HasOne(d => d.DeliveryAddress)
                .WithMany() // Address không cần Navigation Property ngược từ Order
                .HasForeignKey(d => d.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Order_DeliveryAddressId");
            // ⭐ BỔ SUNG: Quan hệ One-to-Many tới OrderPayment
            entity.HasMany(d => d.OrderPayments)
                .WithOne(p => p.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade) // Nếu Order bị xóa (Hard Delete), các giao dịch liên quan cũng bị xóa
                .HasConstraintName("FK_Order_OrderPayments");
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
                .HasConversion<byte>();
            entity.Property(e => e.SugarLevel)
                .HasColumnName("sugar_level")
                .HasConversion<byte>();

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
                .HasConversion<byte>();

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
                .HasConversion<byte>(); // Lưu dưới dạng byte (1, 2, 3...)

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
                .HasConversion<byte>(); // Lưu Enum Status
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
                .HasConversion<byte>()
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
                .HasConversion<byte>()
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
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Status)
                .HasDefaultValue(PolicyReviewStatusEnum.Pending)
                .HasColumnName("status")
                .HasConversion<byte>();
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

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");

            entity.HasKey(e => e.Id).HasName("pk_product");

            // Indexes
            entity.HasIndex(e => e.Slug)
                .IsUnique()
                .HasDatabaseName("ix_product_slug");

            entity.HasIndex(e => e.PublicId)
                .IsUnique()
                .HasDatabaseName("ix_product_public_id");

            // Properties
            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("gen_random_uuid()"); // Postgre function

            entity.Property(e => e.CategoryId)
                .HasColumnName("category_id");

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("slug");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.ProductType)
                .IsRequired()
                .HasMaxLength(50) // Tăng lên 50 cho thoải mái
                .HasColumnName("product_type");

            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("base_price");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("image_url");

            entity.Property(e => e.Description)
                .HasColumnType("text") // Postgre text (unlimited)
                .HasColumnName("description");

            entity.Property(e => e.Ingredient)
                .HasColumnType("text")
                .HasColumnName("ingredient");

            // Enum Status (Byte)
            entity.Property(e => e.Status)
                .HasConversion<byte>()
                .HasDefaultValue(ProductStatusEnum.Active)
                .HasColumnName("status");

            entity.Property(e => e.TotalRating)
                .HasDefaultValue(0.0)
                .HasColumnName("total_rating");

            entity.Property(e => e.TotalSold)
                .HasDefaultValue(0)
                .HasColumnName("total_sold");

            // SearchVector: Tạm thời bỏ qua nếu chưa dùng Full Text Search nâng cao
            // Nếu muốn dùng, cần cài Npgsql.EntityFrameworkCore.PostgreSQL.Search
            entity.Ignore(e => e.SearchVector);

            // Timestamps
            entity.Property(e => e.LaunchDateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("launch_date_time");

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

            // Relationships
            entity.HasOne(d => d.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict) // An toàn: Không cho xóa Category nếu còn Product
                .HasConstraintName("fk_product_category");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            // Đã đúng: Thiết lập khóa kép
            entity.HasKey(p => new { p.ProductId, p.SizeId });

            entity.ToTable("product_size"); // ⭐ Tối ưu: Đảm bảo tên bảng là snake_case

            // ⭐ BỔ SUNG: Quan hệ tới Product
            entity.HasOne(d => d.Product)
                .WithMany(p => p.ProductSizes) // Giả định Product có ICollection<ProductSize>
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade) // Nếu Product bị xóa, các liên kết Size sẽ bị xóa theo
                .HasConstraintName("FK_ProductSize_ProductId");

            // ⭐ BỔ SUNG: Quan hệ tới Size
            entity.HasOne(d => d.Size)
                .WithMany(p => p.ProductSizes) // Giả định Size có ICollection<ProductSize>
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.Cascade) // Nếu Size bị xóa, các liên kết Product sẽ bị xóa theo
                .HasConstraintName("FK_ProductSize_SizeId");
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
                .HasConversion<byte>();
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
            entity.HasKey(e => e.Id).HasName("PK__Review__3213E83F5E4BD5F0");

            entity.ToTable("review");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminResponse).HasColumnName("admin_response");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.MediaUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("media_url");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Status)
                .HasDefaultValue(ReviewStatusEnum.Pending)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__product___7F2BE32F");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Review__user_id__00200768");
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
                .HasConversion<byte>()
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
                .HasConversion<byte>();
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
            entity.HasKey(e => e.Id).HasName("PK__Size__3213E83F985D1A04");

            entity.ToTable("size");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status)
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Label)
                .HasMaxLength(20)
                .HasColumnName("label");
            entity.Property(e => e.PriceModifier)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price_modifier");
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
                
            entity.Property(e => e.PlatformName)
                .HasMaxLength(30)
                .IsUnicode(false)
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
                .HasConversion<byte>();

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
            entity.HasKey(e => e.Id).HasName("PK_Staff_Id");
            entity.ToTable("staff");

            // PublicId tự động tạo
            entity.HasIndex(e => e.PublicId, "UQ_Staff_PublicId").IsUnique();
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("public_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            entity.Property(e => e.FullName).HasMaxLength(100).HasColumnName("full_name");
            entity.Property(e => e.CitizenId).HasMaxLength(20).IsUnicode(false).HasColumnName("citizen_id");
            entity.Property(e => e.Address).HasMaxLength(500).HasColumnName("address");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");

            // Enum Mappings
            entity.Property(e => e.Position)
                .HasColumnName("position")
                .HasConversion<byte>();

            entity.Property(e => e.SalaryType)
                .HasColumnName("salary_type")
                .HasDefaultValue(SalaryTypeEnum.PartTime)
                .HasConversion<byte>();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasConversion<byte>();

            // Salary Fields
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)").HasColumnName("base_salary");
            entity.Property(e => e.HourlySalary).HasColumnType("decimal(18, 2)").HasColumnName("hourly_salary");
            entity.Property(e => e.OvertimeHourlySalary).HasColumnType("decimal(18, 2)").HasColumnName("overtime_hourly_salary");

            // Constraints
            entity.Property(e => e.MinWorkHoursPerMonth).HasColumnName("min_work_hours_per_month");
            entity.Property(e => e.MaxWorkHoursPerMonth).HasColumnName("max_work_hours_per_month");
            entity.Property(e => e.MaxOvertimeHoursPerMonth).HasColumnName("max_overtime_hours_per_month");

            // Audit
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(NOW())").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(NOW())").HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            // Relationships
            // 1-1 với User (Staff phụ thuộc vào User)
            entity.HasOne(d => d.User)
                .WithOne(p => p.Staff) // Cần thêm property Staff vào User.cs
                .HasForeignKey<Staff>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // N-1 với Store (Tùy chọn)
            entity.HasOne(d => d.Store)
                .WithMany(p => p.Staffs) // Cần thêm property Staffs vào Store.cs
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.SetNull); // Nếu Store bị xóa, Staff trở thành nhân viên tự do (hoặc chuyển Store khác)
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Store__3213E83FCF8EAB0E");

            entity.ToTable("store");

            entity.HasIndex(e => e.Slug, "UQ__Store__32DD1E4C92725A74").IsUnique();

            entity.HasIndex(e => e.PublicId, "UQ__Store__5699A53072383001").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(StoreStatusEnum.ComingSoon) 
                .HasConversion<byte>(); // ⭐ Quan trọng: Chuyển đổi Enum thành byte trong DB
            
            entity.Property(e => e.MapVerified)
                .HasDefaultValue(false)
                .HasColumnName("map_verified");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("public_id");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.SortOrder)
                .HasDefaultValue((byte)0)
                .HasColumnName("sort_order");
            entity.Property(e => e.OpenDate)
                .HasColumnType("timestamp without time zone") // Hoặc "date" nếu chỉ cần ngày
                .HasColumnName("open_date");
            entity.HasOne(d => d.Brand).WithMany(p => p.Stores)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Store__brand_id__5535A963");
            // Bổ sung khóa ngoại AddressId
            entity.Property(e => e.AddressId).HasColumnName("address_id");

            // Bổ sung các trường phí ship
            entity.Property(e => e.ShippingFeeFixed)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)") // Phí cố định
                .HasColumnName("shipping_fee_fixed");

            entity.Property(e => e.ShippingFeePerKm)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)") // Phí trên mỗi KM
                .HasColumnName("shipping_fee_per_km");

            // Thiết lập quan hệ 1-N: Một Address có thể được sử dụng bởi nhiều Store (ít phổ biến nhưng linh hoạt)
            // Hoặc 1-1: Một Store có một Address duy nhất (Nếu đây là Store chính)
            entity.HasOne(d => d.Address) // NP trong Store
                .WithOne(p => p.Store) // ⭐ CHỈ ĐỊNH NP ngược trong Address
                .HasForeignKey<Store>(d => d.AddressId) // Khóa ngoại nằm ở Store
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_Store_AddressId");
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
                .HasConversion<byte>();

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
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F0D985B4B");

            entity.ToTable("user");

            entity.HasIndex(e => e.PublicId, "UQ__User__5699A530AE2F5693").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__AB6E616418E7B215").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");   
            entity.Property(e => e.CurrentCoins)
                .HasDefaultValue(0)
                .HasColumnName("current_coins");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerified)
                .HasDefaultValue(false)
                .HasColumnName("email_verified");
            entity.Property(e => e.LastLogin)
                .HasColumnName("last_login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("public_id");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(UserRoleEnum.Customer)
                .HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasDefaultValue(UserStatusEnum.Active)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("thumbnail_url");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
            // ⭐ CÁC THUỘC TÍNH AUTH MỚI THÊM
            entity.Property(e => e.RefreshToken)
                .HasColumnName("refresh_token")
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.RefreshTokenExpiryTime)
                .HasColumnName("refresh_token_expiry_time")
                .HasColumnType("timestamp without time zone");

            entity.Property(e => e.ResetPasswordToken)
                .HasColumnName("reset_password_token")
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.ResetPasswordTokenExpiryTime)
                .HasColumnName("reset_password_token_expiry_time")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<UserVoucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_Vou__3213E83F950C18C0");

            entity.ToTable("user_Voucher");

            entity.HasIndex(e => e.VoucherCode, "UQ__User_Vou__21731069F90ADEF0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.ExpiryDate)
                .HasColumnName("expiry_date");
            entity.Property(e => e.IssuedDate)
                .HasDefaultValueSql("(NOW())").HasColumnType("timestamp without time zone")
                .HasColumnName("issued_date");
            entity.Property(e => e.OrderIdUsed).HasColumnName("order_id_used");
            
            entity.Property(e => e.Status)
                .HasDefaultValue(UserVoucherStatusEnum.Unused)
                .HasColumnName("status")
                .HasConversion<byte>();

            entity.Property(e => e.UsedDate)
                .HasColumnName("used_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("voucher_code");
            entity.Property(e => e.VoucherTemplateId).HasColumnName("voucher_template_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Vouc__order__65370702");

            entity.HasOne(d => d.VoucherTemplate).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.VoucherTemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Vouc__vouch__662B2B3B");

            entity.HasOne(d => d.OrderUsed) // Giả định bạn có Navigation Property là OrderUsed trong Model
                .WithMany() // Order không cần Navigation Property ngược từ UserVoucher
                .HasForeignKey(d => d.OrderIdUsed)
                .IsRequired(false) // Cho phép OrderIdUsed là null (khi chưa sử dụng)
                .OnDelete(DeleteBehavior.Restrict) // Không cho xóa Order nếu UserVoucher đã sử dụng nó
                .HasConstraintName("FK_UserVoucher_OrderIdUsed");
        });

        modelBuilder.Entity<VoucherTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Voucher___3213E83F0298D293");

            entity.ToTable("voucher_Template");

            entity.HasIndex(e => e.CouponCode, "UQ__Voucher___ADE5CBB794CB76DB").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CouponCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("coupon_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NOW())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate)
                .HasColumnName("end_date");
            entity.Property(e => e.Status)
                .HasDefaultValue(PublicStatusEnum.Active)
                .HasColumnName("status")
                .HasConversion<byte>();
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.MaxDiscountAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("max_discount_amount");
            entity.Property(e => e.MinOrderValue)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("min_order_value");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.QuantityPerLevel).HasColumnName("quantity_per_level");
            entity.Property(e => e.StartDate)
                .HasColumnName("start_date");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            entity.Property(e => e.UsageLimitPerUser).HasColumnName("usage_limit_per_user");
            entity.Property(e => e.UsedCount)
                .HasDefaultValue(0)
                .HasColumnName("used_count");

            entity.HasOne(d => d.Level).WithMany(p => p.VoucherTemplates)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK__Voucher_T__level__5E8A0973");
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
