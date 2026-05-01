namespace drinking_be.Utils
{
    public static class Utils
    {
        public static string GetIpAddress(HttpContext context)
        {
            if (context == null) return "127.0.0.1";

            string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            // Bắt trường hợp chạy sau Proxy/Load Balancer (Nginx, IIS...)
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? ipAddress;
            }

            // Chuyển đổi IPv6 Localhost (::1) về IPv4
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }
    }
}
