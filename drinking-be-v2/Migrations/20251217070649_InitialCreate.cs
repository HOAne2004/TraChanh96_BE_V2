using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    image_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    link_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    position = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banner_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    hotline = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    email_support = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    tax_code = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: true),
                    company_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    slogan = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    copyright_text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    established_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Brand__3213E83F3D8FA9ED", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    slug = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true, defaultValue: (byte)0),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__3213E83F5A39BDEA", x => x.id);
                    table.ForeignKey(
                        name: "FK__Category__parent__3C69FB99",
                        column: x => x.parent_id,
                        principalTable: "category",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "material",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    base_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    purchase_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    conversion_rate = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    cost_per_purchase_unit = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    min_stock_alert = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material_Id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "membership_level",
                columns: table => new
                {
                    id = table.Column<byte>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    min_spend_required = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    duration_days = table.Column<short>(type: "smallint", nullable: false),
                    benefits = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Membersh__3213E83F3901FC68", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_method",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    payment_type = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    bank_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bank_account_number = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    bank_account_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    instructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    qr_tpl_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
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
                    label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    price_modifier = table.Column<decimal>(type: "numeric(18,2)", nullable: true, defaultValue: 0m),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Size__3213E83F985D1A04", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: true, defaultValueSql: "gen_random_uuid()"),
                    role_id = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    username = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    thumbnail_url = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    current_coins = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    email_verified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reset_password_token = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    reset_password_token_expiry_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__3213E83F0D985B4B", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<string>(type: "character varying(36)", unicode: false, maxLength: 36, nullable: false, defaultValueSql: "gen_random_uuid()"),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    product_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    ingredient = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    total_rating = table.Column<double>(type: "double precision", nullable: true, defaultValue: 0.0),
                    total_sold = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    launch_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_category",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "voucher_template",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    level_id = table.Column<byte>(type: "smallint", nullable: true),
                    discount_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    discount_type = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    min_order_value = table.Column<decimal>(type: "numeric(18,2)", nullable: true, defaultValue: 0m),
                    max_discount_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    quantity_per_level = table.Column<byte>(type: "smallint", nullable: true),
                    usage_limit = table.Column<int>(type: "integer", nullable: true),
                    used_count = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    usage_limit_per_user = table.Column<byte>(type: "smallint", nullable: true),
                    coupon_code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Voucher___3213E83F0298D293", x => x.id);
                    table.ForeignKey(
                        name: "FK__Voucher_T__level__5E8A0973",
                        column: x => x.level_id,
                        principalTable: "membership_level",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    recipient_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    recipient_phone = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    full_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    address_detail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    commune = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address_Id", x => x.id);
                    table.ForeignKey(
                        name: "FK_Address_UserId",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart__3213E83FED0FE127", x => x.id);
                    table.ForeignKey(
                        name: "FK__Cart__user_id__37703C52",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
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
                    card_code = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    level_id = table.Column<byte>(type: "smallint", nullable: false),
                    total_spent = table.Column<decimal>(type: "numeric(12,2)", nullable: true, defaultValue: 0m),
                    level_start_date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    level_end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    last_level_spent_reset = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Membersh__3213E83FF483D767", x => x.id);
                    table.ForeignKey(
                        name: "FK__Membershi__level__51300E55",
                        column: x => x.level_id,
                        principalTable: "membership_level",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Membershi__user___503BEA1C",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "news",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: true, defaultValueSql: "gen_random_uuid()"),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    thumbnail_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    is_featured = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    seo_description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    published_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__News__3213E83F6A9F9780", x => x.id);
                    table.ForeignKey(
                        name: "FK__News__user_id__73BA3083",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
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
                    type = table.Column<byte>(type: "smallint", nullable: false),
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
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SizeId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_size", x => new { x.ProductId, x.SizeId });
                    table.ForeignKey(
                        name: "FK_ProductSize_ProductId",
                        column: x => x.ProductId,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSize_SizeId",
                        column: x => x.SizeId,
                        principalTable: "size",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<byte>(type: "smallint", nullable: false),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    media_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    admin_response = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review__3213E83F5E4BD5F0", x => x.id);
                    table.ForeignKey(
                        name: "FK__Review__product___7F2BE32F",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Review__user_id__00200768",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: true, defaultValueSql: "gen_random_uuid()"),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    brand_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    image_url = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: true),
                    open_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    open_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    close_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    shipping_fee_fixed = table.Column<decimal>(type: "numeric(10,2)", nullable: true, defaultValue: 0m),
                    shipping_fee_per_km = table.Column<decimal>(type: "numeric(10,2)", nullable: true, defaultValue: 0m),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true, defaultValue: (byte)0),
                    map_verified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    address_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Store__3213E83FCF8EAB0E", x => x.id);
                    table.ForeignKey(
                        name: "FK_Store_AddressId",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Store__brand_id__5535A963",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cart_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cart_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    final_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    parent_item_id = table.Column<long>(type: "bigint", nullable: true),
                    size_id = table.Column<short>(type: "smallint", nullable: true),
                    sugar_level = table.Column<byte>(type: "smallint", nullable: true),
                    ice_level = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart_ite__3213E83F486E79A8", x => x.id);
                    table.ForeignKey(
                        name: "FK__Cart_item__cart___3B40CD36",
                        column: x => x.cart_id,
                        principalTable: "cart",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Cart_item__paren__3D2915A8",
                        column: x => x.parent_item_id,
                        principalTable: "cart_item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Cart_item__produ__3C34F16F",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Cart_item__size___3E1D39E1",
                        column: x => x.size_id,
                        principalTable: "size",
                        principalColumn: "id");
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
                    content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comment__3213E83FE8B13CD2", x => x.id);
                    table.ForeignKey(
                        name: "FK__Comment__news_id__06CD04F7",
                        column: x => x.news_id,
                        principalTable: "news",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Comment__parent___04E4BC85",
                        column: x => x.parent_id,
                        principalTable: "comment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Comment__user_id__05D8E0BE",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "inventory",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    material_id = table.Column<int>(type: "integer", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(NOW())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_Id", x => x.id);
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_code = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    payment_method_id = table.Column<int>(type: "integer", nullable: true),
                    delivery_address_id = table.Column<long>(type: "bigint", nullable: false),
                    order_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    delivery_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true, defaultValue: 0m),
                    shipping_fee = table.Column<decimal>(type: "numeric(8,2)", nullable: true, defaultValue: 0m),
                    grand_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    coins_earned = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    voucher_code_used = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    store_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    user_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AddressId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order__3213E83FDE5887F1", x => x.id);
                    table.ForeignKey(
                        name: "FK_Order_DeliveryAddressId",
                        column: x => x.delivery_address_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_PaymentMethodId",
                        column: x => x.payment_method_id,
                        principalTable: "payment_method",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Order__store_id__282DF8C2",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Order__user_id__2739D489",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_order_address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "address",
                        principalColumn: "id");
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
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
                    brand_id = table.Column<int>(type: "integer", nullable: false),
                    store_id = table.Column<int>(type: "integer", nullable: false),
                    platform_name = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", unicode: false, maxLength: 500, nullable: true),
                    sort_order = table.Column<byte>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2)
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
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    citizen_id = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    position = table.Column<byte>(type: "smallint", nullable: false),
                    hire_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    salary_type = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    base_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    hourly_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    overtime_hourly_salary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    min_work_hours_per_month = table.Column<double>(type: "double precision", nullable: true),
                    max_work_hours_per_month = table.Column<double>(type: "double precision", nullable: true),
                    max_overtime_hours_per_month = table.Column<double>(type: "double precision", nullable: true),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
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
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_staff_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
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
                    sugar_level = table.Column<byte>(type: "smallint", nullable: false),
                    ice_level = table.Column<byte>(type: "smallint", nullable: false)
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
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
                name: "user_voucher",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    voucher_template_id = table.Column<int>(type: "integer", nullable: false),
                    voucher_code = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    issued_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "(NOW())"),
                    expiry_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    used_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    order_id_used = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Vou__3213E83F950C18C0", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserVoucher_OrderIdUsed",
                        column: x => x.order_id_used,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__User_Vouc__order__65370702",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__User_Vouc__vouch__662B2B3B",
                        column: x => x.voucher_template_id,
                        principalTable: "voucher_template",
                        principalColumn: "id");
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)2),
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)5),
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
                    applied_salary_type = table.Column<byte>(type: "smallint", nullable: false),
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
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
                    status = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
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
                name: "IX_banner_position_status_order",
                table: "banner",
                columns: new[] { "position", "status", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "IX_banner_sort_order",
                table: "banner",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "UQ__Cart__B9BE370E2DCD722C",
                table: "cart",
                column: "user_id",
                unique: true);

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
                name: "UQ__Category__32DD1E4C00F59B8C",
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
                name: "IX_inventory_store_id",
                table: "inventory",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Inventory_Material_Store",
                table: "inventory",
                columns: new[] { "material_id", "store_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_material_public_id",
                table: "material",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_membership_level_id",
                table: "membership",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Membersh__81703D727EAE3828",
                table: "membership",
                column: "card_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Membersh__B9BE370EFC41B82F",
                table: "membership",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Membersh__72E12F1BBCF20D34",
                table: "membership_level",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_news_user_id",
                table: "news",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__News__32DD1E4C5A174899",
                table: "news",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__News__5699A53062598161",
                table: "news",
                column: "public_id",
                unique: true);

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
                name: "IX_order_payment_method_id",
                table: "order",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_store_id",
                table: "order",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_user_id",
                table: "order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Order__99D12D3FD8ECB07F",
                table: "order",
                column: "order_code",
                unique: true);

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
                name: "IX_product_category_id",
                table: "product",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_public_id",
                table: "product",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_slug",
                table: "product",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_size_SizeId",
                table: "product_size",
                column: "SizeId");

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
                name: "IX_review_user_id",
                table: "review",
                column: "user_id");

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
                name: "IX_staff_store_id",
                table: "staff",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_id",
                table: "staff",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Staff_PublicId",
                table: "staff",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_address_id",
                table: "store",
                column: "address_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_brand_id",
                table: "store",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Store__32DD1E4C92725A74",
                table: "store",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Store__5699A53072383001",
                table: "store",
                column: "public_id",
                unique: true);

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
                name: "UQ__User__5699A530AE2F5693",
                table: "user",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__User__AB6E616418E7B215",
                table: "user",
                column: "email",
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
                name: "IX_user_voucher_voucher_template_id",
                table: "user_voucher",
                column: "voucher_template_id");

            migrationBuilder.CreateIndex(
                name: "UQ__User_Vou__21731069F90ADEF0",
                table: "user_voucher",
                column: "voucher_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_voucher_template_level_id",
                table: "voucher_template",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Voucher___ADE5CBB794CB76DB",
                table: "voucher_template",
                column: "coupon_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "membership");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "order_payment");

            migrationBuilder.DropTable(
                name: "payslip");

            migrationBuilder.DropTable(
                name: "policy");

            migrationBuilder.DropTable(
                name: "product_size");

            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "social_media");

            migrationBuilder.DropTable(
                name: "supply_order_item");

            migrationBuilder.DropTable(
                name: "user_voucher");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "news");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "size");

            migrationBuilder.DropTable(
                name: "shop_table");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "material");

            migrationBuilder.DropTable(
                name: "supply_order");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "voucher_template");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "payment_method");

            migrationBuilder.DropTable(
                name: "membership_level");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
