using drinking_be.Data;
using drinking_be.Interfaces;
using drinking_be.Interfaces.AuthInterfaces;
using drinking_be.Interfaces.FeedbackInterfaces;
using drinking_be.Interfaces.MarketingInterfaces;
using drinking_be.Interfaces.OptionInterfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Interfaces.PolicyInterfaces;
using drinking_be.Interfaces.ProductInterfaces;
using drinking_be.Interfaces.StoreInterfaces;
using drinking_be.Models;
using drinking_be.Repositories;
using drinking_be.Services;
using drinking_be.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using Supabase;

// Fix lỗi timestamp của PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --- 1. CẤU HÌNH DATABASE ---
var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"👉 [DEBUG CHECK] Connection String: '{dbConnectionString}'");

builder.Services.AddDbContext<DBDrinkContext>(options =>
{
    options.UseNpgsql(dbConnectionString);
});

// --- 2. CẤU HÌNH CƠ BẢN ---
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Drinking API", Version = "v1" });

    // Cấu hình nút Authorize (Ổ khóa)
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nhập Token vào ô bên dưới (Không cần chữ Bearer, chỉ cần dán token)",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddHostedService<drinking_be.Services.Background.SoftDeleteCleanupService>();

// ==================================================================
// 3. ĐĂNG KÝ DEPENDENCY INJECTION (DI)
// ==================================================================

// --- A. CORE & UTILS ---
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IEnumOptionService, EnumOptionService>();

// --- B. AUTH & USERS ---
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // (Nếu Auth vẫn dùng riêng Repo)

// --- C. STORE & STAFF ---
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStaffService, StaffService>(); // ✅ Đã thêm
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPayslipService, PayslipService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IShopTableService, ShopTableService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// --- D. PRODUCT & INVENTORY ---
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ISupplyOrderService, SupplyOrderService>();

// --- E. ORDER & PAYMENT ---
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>(); // ✅ Đã xóa trùng

// --- F. MARKETING & CONTENT ---
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<ISocialMediaService, SocialMediaService>();
builder.Services.AddScoped<IFranchiseService, FranchiseService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// --- G. MEMBERSHIP & LOYALTY ---
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IMembershipLevelService, MembershipLevelService>();
builder.Services.AddScoped<IVoucherTemplateService, VoucherTemplateService>();
builder.Services.AddScoped<IUserVoucherService, UserVoucherService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IBrandService, BrandService>();

// ==================================================================

// --- 4. CẤU HÌNH JWT AUTH ---
var config = builder.Configuration;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = true,
        ValidIssuer = config["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = config["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// --- 5. CẤU HÌNH CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.SetIsOriginAllowed(origin => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// --- 6. PIPELINE ---
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

// --- 7. MIGRATION & SEED DATA ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DBDrinkContext>();
        await context.Database.MigrateAsync();
        await DbInitializer.SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi khởi tạo Database.");
    }
}

// --- 8. CẤU HÌNH SUPABASE ---
var url = builder.Configuration["Supabase:Url"];
var key_supabase = builder.Configuration["Supabase:Key"];

// Đăng ký Supabase Client (Scoped hoặc Singleton đều được)
builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(url, key_supabase, new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    }));


app.MapControllers();
app.Run();