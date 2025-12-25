using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltySystemManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banner",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    link_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    start_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    end_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_clickable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banner", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    hotline = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email_support = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tax_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    slogan = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    copyright_text = table.Column<string>(type: "text", nullable: true),
                    established_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_category_category_parent_id",
                        column: x => x.parent_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "material",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    base_unit = table.Column<short>(type: "smallint", nullable: false),
                    purchase_unit = table.Column<short>(type: "smallint", nullable: true),
                    conversion_rate = table.Column<int>(type: "integer", nullable: false),
                    cost_per_purchase_unit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    min_stock_alert = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_material", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "membership_level",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rank_order = table.Column<short>(type: "smallint", nullable: false),
                    min_coins_required = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PointEarningRate = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    ResetReductionPercent = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    duration_days = table.Column<int>(type: "integer", nullable: true),
                    benefits = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipLevel_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_method",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    payment_type = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    bank_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bank_account_number = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    bank_account_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    instructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    qr_tpl_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true),
                    processing_fee = table.Column<decimal>(type: "numeric(18,2)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "size",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    price_modifier = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Size_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    role_id = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    thumbnail_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    last_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    refresh_token_expiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reset_password_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    reset_password_token_expiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    product_type = table.Column<short>(type: "smallint", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    ingredient = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    total_rating = table.Column<double>(type: "double precision", nullable: true),
                    total_sold = table.Column<int>(type: "integer", nullable: true),
                    search_vector = table.Column<byte[]>(type: "bytea", nullable: true),
                    launch_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "voucher_template",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    membership_level_id = table.Column<int>(type: "integer", nullable: true),
                    discount_type = table.Column<short>(type: "smallint", nullable: false),
                    discount_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    min_order_value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    max_discount_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    total_quantity = table.Column<int>(type: "integer", nullable: true),
                    used_count = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    usage_limit_per_user = table.Column<int>(type: "integer", nullable: true),
                    coupon_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherTemplate_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_voucher_template_membership_level_membership_level_id",
                        column: x => x.membership_level_id,
                        principalTable: "membership_level",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    recipient_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    recipient_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    full_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    address_detail = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    commune = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    status = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_address_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "franchise_request",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    target_area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    estimated_budget = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    experience_description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    admin_note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    reviewer_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FranchiseRequest_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_franchise_request_reviewer",
                        column: x => x.reviewer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "membership",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    membership_level_id = table.Column<int>(type: "integer", nullable: false),
                    card_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    current_coins = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_spent = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    level_start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    level_end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    last_level_spent_reset = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membership_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_membership_membership_level_membership_level_id",
                        column: x => x.membership_level_id,
                        principalTable: "membership_level",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_membership_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "news",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    slug = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    thumbnail_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    view_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    seo_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    published_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_news", x => x.id);
                    table.ForeignKey(
                        name: "FK_news_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    reference_id = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    scheduled_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(NOW())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_size",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    size_id = table.Column<short>(type: "smallint", nullable: false),
                    price_override = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSize", x => new { x.product_id, x.size_id });
                    table.ForeignKey(
                        name: "FK_product_size_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_size_size_size_id",
                        column: x => x.size_id,
                        principalTable: "size",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    rating = table.Column<byte>(type: "smallint", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    media_url = table.Column<string>(type: "text", nullable: true),
                    admin_response = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review", x => x.id);
                    table.ForeignKey(
                        name: "FK_review_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_review_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BrandId = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    open_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    open_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    close_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    shipping_fee_fixed = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    shipping_fee_per_km = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true),
                    map_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    address_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_store_address_address_id",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_store_brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brand",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    news_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_comment_parent_id",
                        column: x => x.parent_id,
                        principalTable: "comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comment_news_news_id",
                        column: x => x.news_id,
                        principalTable: "news",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comment_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart", x => x.id);
                    table.ForeignKey(
                        name: "FK_cart_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cart_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    material_id = table.Column<int>(type: "integer", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_material_material_id",
                        column: x => x.material_id,
                        principalTable: "material",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventory_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "policy",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    brand_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Policy__3213E83FB49E512D", x => x.id);
                    table.ForeignKey(
                        name: "FK_Policy_StoreId",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Policy__brand_id__5BE2A6F2",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "product_store",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    sold_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    price_override = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStore", x => new { x.product_id, x.store_id });
                    table.ForeignKey(
                        name: "FK_product_store_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_store_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    capacity = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    is_air_conditioned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_smoking_allowed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_td", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_store",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "social_media",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand_id = table.Column<int>(type: "integer", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true),
                    platform_name = table.Column<short>(type: "smallint", nullable: false),
                    url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMedia_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_SocialMedia_BrandId",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_SocialMedia_StoreId",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: true),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    citizen_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    position = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)13),
                    hire_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    salary_type = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    base_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    hourly_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    overtime_hourly_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    min_work_hours_per_month = table.Column<double>(type: "double precision", nullable: true),
                    max_work_hours_per_month = table.Column<double>(type: "double precision", nullable: true),
                    max_overtime_hours_per_month = table.Column<double>(type: "double precision", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_staff_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_staff_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supply_order",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    order_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: true),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    approved_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    expected_delivery_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    received_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyOrder_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_supply_order_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_supply_order_user_approved_by_user_id",
                        column: x => x.approved_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supply_order_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cart_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cart_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    size_id = table.Column<short>(type: "smallint", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    final_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    sugar_level = table.Column<short>(type: "smallint", nullable: true),
                    ice_level = table.Column<short>(type: "smallint", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    parent_item_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_cart_item_cart_cart_id",
                        column: x => x.cart_id,
                        principalTable: "cart",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cart_item_cart_item_parent_item_id",
                        column: x => x.parent_item_id,
                        principalTable: "cart_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cart_item_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cart_item_size_size_id",
                        column: x => x.size_id,
                        principalTable: "size",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shop_table",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    capacity = table.Column<byte>(type: "smallint", nullable: false),
                    can_be_merged = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    merged_with_table_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Table__3213E83FCBCE513F", x => x.id);
                    table.ForeignKey(
                        name: "FK__Table__merged_wi__0D44F85C",
                        column: x => x.merged_with_table_id,
                        principalTable: "shop_table",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Table__store_id__0C50D423",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_shop_table_room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "room",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "attendance",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    staff_id = table.Column<int>(type: "integer", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    check_in_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    check_out_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    working_hours = table.Column<double>(type: "double precision", nullable: false),
                    overtime_hours = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)5),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    daily_bonus = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    daily_deduction = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendance_staff_staff_id",
                        column: x => x.staff_id,
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attendance_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payslip",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    staff_id = table.Column<int>(type: "integer", nullable: false),
                    month = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    from_date = table.Column<DateOnly>(type: "date", nullable: false),
                    to_date = table.Column<DateOnly>(type: "date", nullable: false),
                    applied_salary_type = table.Column<short>(type: "smallint", nullable: false),
                    applied_base_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    applied_hourly_rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    applied_overtime_rate = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    total_work_hours = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    total_overtime_hours = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    total_work_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    salary_before_tax = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    allowance = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    bonus = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    deduction = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    final_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslip_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_payslip_staff",
                        column: x => x.staff_id,
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supply_order_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    supply_order_id = table.Column<long>(type: "bigint", nullable: false),
                    material_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cost_per_unit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyOrderItem_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_supply_order_item_material_material_id",
                        column: x => x.material_id,
                        principalTable: "material",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_supply_order_item_supply_order_supply_order_id",
                        column: x => x.supply_order_id,
                        principalTable: "supply_order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reservation_code = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    reservation_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    number_of_guests = table.Column<byte>(type: "smallint", nullable: false),
                    customer_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    customer_phone = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    assigned_table_id = table.Column<int>(type: "integer", nullable: true),
                    deposit_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    is_deposit_paid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation", x => x.id);
                    table.ForeignKey(
                        name: "FK__Reservati__assig__17C286CF",
                        column: x => x.assigned_table_id,
                        principalTable: "shop_table",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Reservati__store__16CE6296",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Reservati__user___15DA3E5D",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_code = table.Column<string>(type: "text", nullable: false),
                    pickup_code = table.Column<string>(type: "text", nullable: true),
                    order_type = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    shipper_id = table.Column<int>(type: "integer", nullable: true),
                    table_id = table.Column<int>(type: "integer", nullable: true),
                    payment_method_id = table.Column<int>(type: "integer", nullable: true),
                    delivery_address_id = table.Column<long>(type: "bigint", nullable: true),
                    user_voucher_id = table.Column<long>(type: "bigint", nullable: true),
                    order_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    delivery_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    shipping_fee = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    grand_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    coins_earned = table.Column<int>(type: "integer", nullable: true),
                    store_name = table.Column<string>(type: "text", nullable: true),
                    voucher_code_used = table.Column<string>(type: "text", nullable: true),
                    user_notes = table.Column<string>(type: "text", nullable: true),
                    cancel_reason = table.Column<short>(type: "smallint", nullable: true),
                    cancel_note = table.Column<string>(type: "text", nullable: true),
                    cancelled_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    AddressId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_Order_PaymentMethodId",
                        column: x => x.payment_method_id,
                        principalTable: "payment_method",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "address",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_order_address_delivery_address_id",
                        column: x => x.delivery_address_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_shop_table_table_id",
                        column: x => x.table_id,
                        principalTable: "shop_table",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_order_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_user_shipper_id",
                        column: x => x.shipper_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    final_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    parent_item_id = table.Column<long>(type: "bigint", nullable: true),
                    size_id = table.Column<short>(type: "smallint", nullable: true),
                    sugar_level = table.Column<short>(type: "smallint", nullable: false),
                    ice_level = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_it__3213E83F36FD5A45", x => x.id);
                    table.ForeignKey(
                        name: "FK__Order_ite__order__2CF2ADDF",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Order_ite__paren__2EDAF651",
                        column: x => x.parent_item_id,
                        principalTable: "order_item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Order_ite__produ__2DE6D218",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Order_ite__size___2FCF1A8A",
                        column: x => x.size_id,
                        principalTable: "size",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order_payment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    payment_method_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    transaction_code = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    payment_signature = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    payment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPayment_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderPayment_OrderId",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPayment_PaymentMethodId",
                        column: x => x.payment_method_id,
                        principalTable: "payment_method",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "point_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    membership_id = table.Column<long>(type: "bigint", nullable: false),
                    order_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    balance_after = table.Column<int>(type: "integer", nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointHistory_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_point_history_membership_membership_id",
                        column: x => x.membership_id,
                        principalTable: "membership",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_point_history_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_voucher",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    voucher_template_id = table.Column<int>(type: "integer", nullable: false),
                    voucher_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    issued_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    expiry_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    used_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    order_id_used = table.Column<long>(type: "bigint", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVoucher_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_voucher_order_order_id_used",
                        column: x => x.order_id_used,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_user_voucher_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_voucher_voucher_template_voucher_template_id",
                        column: x => x.voucher_template_id,
                        principalTable: "voucher_template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_address_user_id",
                table: "address",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_staff_id",
                table: "attendance",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_store_id",
                table: "attendance",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_banner_position_status",
                table: "banner",
                columns: new[] { "position", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_brand_public_id",
                table: "brand",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_store_id",
                table: "cart",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_user_id",
                table: "cart",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_user_id_status",
                table: "cart",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_cart_id",
                table: "cart_item",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_parent_item_id",
                table: "cart_item",
                column: "parent_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_product_id",
                table: "cart_item",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_size_id",
                table: "cart_item",
                column: "size_id");

            migrationBuilder.CreateIndex(
                name: "IX_category_parent_id",
                table: "category",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_category_slug",
                table: "category",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comment_news_id",
                table: "comment",
                column: "news_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_parent_id",
                table: "comment",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_user_id",
                table: "comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_franchise_request_reviewer_id",
                table: "franchise_request",
                column: "reviewer_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_material_id_store_id",
                table: "inventory",
                columns: new[] { "material_id", "store_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inventory_store_id",
                table: "inventory",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_membership_card_code",
                table: "membership",
                column: "card_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_membership_membership_level_id",
                table: "membership",
                column: "membership_level_id");

            migrationBuilder.CreateIndex(
                name: "IX_membership_user_id",
                table: "membership",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_membership_level_rank_order",
                table: "membership_level",
                column: "rank_order",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_news_public_id",
                table: "news",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_news_slug",
                table: "news",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_news_UserId",
                table: "news",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_created_at",
                table: "notification",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_id",
                table: "notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_read",
                table: "notification",
                columns: new[] { "user_id", "is_read" });

            migrationBuilder.CreateIndex(
                name: "IX_order_AddressId",
                table: "order",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_order_delivery_address_id",
                table: "order",
                column: "delivery_address_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_order_code",
                table: "order",
                column: "order_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_method_id",
                table: "order",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_pickup_code",
                table: "order",
                column: "pickup_code");

            migrationBuilder.CreateIndex(
                name: "IX_order_shipper_id",
                table: "order",
                column: "shipper_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_store_id",
                table: "order",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_table_id",
                table: "order",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_user_id",
                table: "order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_user_voucher_id",
                table: "order",
                column: "user_voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_order_id",
                table: "order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_parent_item_id",
                table: "order_item",
                column: "parent_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_product_id",
                table: "order_item",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_size_id",
                table: "order_item",
                column: "size_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_order_id",
                table: "order_payment",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_payment_method_id",
                table: "order_payment",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_payslip_staff_month_year",
                table: "payslip",
                columns: new[] { "staff_id", "month", "year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_point_history_created_at",
                table: "point_history",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_point_history_membership_id",
                table: "point_history",
                column: "membership_id");

            migrationBuilder.CreateIndex(
                name: "IX_point_history_order_id",
                table: "point_history",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_point_history_transaction_type",
                table: "point_history",
                column: "transaction_type");

            migrationBuilder.CreateIndex(
                name: "IX_policy_brand_id",
                table: "policy",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_policy_store_id",
                table: "policy",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Policy__32DD1E4C980885E3",
                table: "policy",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_CategoryId",
                table: "product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_product_public_id",
                table: "product",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_size_size_id",
                table: "product_size",
                column: "size_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_store_store_id",
                table: "product_store",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_assigned_table_id",
                table: "reservation",
                column: "assigned_table_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_store_id",
                table: "reservation",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_user_id",
                table: "reservation",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Reservat__FA8FADE431DEB25F",
                table: "reservation",
                column: "reservation_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_review_product_id",
                table: "review",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_user_id_product_id",
                table: "review",
                columns: new[] { "user_id", "product_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_store_id",
                table: "room",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_shop_table_merged_with_table_id",
                table: "shop_table",
                column: "merged_with_table_id");

            migrationBuilder.CreateIndex(
                name: "IX_shop_table_RoomId",
                table: "shop_table",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_shop_table_store_id",
                table: "shop_table",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_social_media_brand_id",
                table: "social_media",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_social_media_store_id",
                table: "social_media",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_public_id",
                table: "staff",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_staff_store_id",
                table: "staff",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_id",
                table: "staff",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_address_id",
                table: "store",
                column: "address_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_BrandId",
                table: "store",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_store_public_id",
                table: "store",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_slug",
                table: "store",
                column: "slug",
                unique: true,
                filter: "slug IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_supply_order_approved_by_user_id",
                table: "supply_order",
                column: "approved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_order_created_by_user_id",
                table: "supply_order",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_order_order_code",
                table: "supply_order",
                column: "order_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supply_order_public_id",
                table: "supply_order",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supply_order_store_id",
                table: "supply_order",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_order_item_material_id",
                table: "supply_order_item",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_order_item_supply_order_id",
                table: "supply_order_item",
                column: "supply_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_public_id",
                table: "user",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                table: "user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_voucher_order_id_used",
                table: "user_voucher",
                column: "order_id_used");

            migrationBuilder.CreateIndex(
                name: "IX_user_voucher_user_id",
                table: "user_voucher",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_voucher_voucher_code",
                table: "user_voucher",
                column: "voucher_code",
                unique: true,
                filter: "voucher_code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_voucher_voucher_template_id",
                table: "user_voucher",
                column: "voucher_template_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_template_coupon_code",
                table: "voucher_template",
                column: "coupon_code",
                unique: true,
                filter: "coupon_code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_template_membership_level_id",
                table: "voucher_template",
                column: "membership_level_id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_user_voucher_user_voucher_id",
                table: "order",
                column: "user_voucher_id",
                principalTable: "user_voucher",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_address_user_user_id",
                table: "address");

            migrationBuilder.DropForeignKey(
                name: "FK_order_user_shipper_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_user_user_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_user_voucher_user_user_id",
                table: "user_voucher");

            migrationBuilder.DropForeignKey(
                name: "FK_order_store_store_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "fk_room_store",
                table: "room");

            migrationBuilder.DropForeignKey(
                name: "FK__Table__store_id__0C50D423",
                table: "shop_table");

            migrationBuilder.DropForeignKey(
                name: "FK_voucher_template_membership_level_membership_level_id",
                table: "voucher_template");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentMethodId",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_address_AddressId",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_address_delivery_address_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_shop_table_table_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_user_voucher_user_voucher_id",
                table: "order");

            migrationBuilder.DropTable(
                name: "attendance");

            migrationBuilder.DropTable(
                name: "banner");

            migrationBuilder.DropTable(
                name: "cart_item");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "franchise_request");

            migrationBuilder.DropTable(
                name: "inventory");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "order_payment");

            migrationBuilder.DropTable(
                name: "payslip");

            migrationBuilder.DropTable(
                name: "point_history");

            migrationBuilder.DropTable(
                name: "policy");

            migrationBuilder.DropTable(
                name: "product_size");

            migrationBuilder.DropTable(
                name: "product_store");

            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "social_media");

            migrationBuilder.DropTable(
                name: "supply_order_item");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "news");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "membership");

            migrationBuilder.DropTable(
                name: "size");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "material");

            migrationBuilder.DropTable(
                name: "supply_order");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "membership_level");

            migrationBuilder.DropTable(
                name: "payment_method");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "shop_table");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "user_voucher");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "voucher_template");
        }
    }
}
