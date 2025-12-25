using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

namespace drinking_be.Models
{
    public class DBDrinkContextFactory : IDesignTimeDbContextFactory<DBDrinkContext>
    {
        public DBDrinkContext CreateDbContext(string[] args)
        {
            var basePath = AppContext.BaseDirectory;
            Console.WriteLine($"[EF MIGRATION] BasePath: {basePath}");

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = config.GetConnectionString("MigrationConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("MigrationConnection is missing.");

            Console.WriteLine($"[EF MIGRATION] Using connection: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<DBDrinkContext>();

            // 👇 SỬA ĐOẠN NÀY: Thêm .UseSnakeCaseNamingConvention()
            optionsBuilder
                .UseNpgsql(connectionString);
                //.UseSnakeCaseNamingConvention(); // <--- DÒNG CÒN THIẾU CỰC KỲ QUAN TRỌNG

            return new DBDrinkContext(optionsBuilder.Options);
        }
    }
}