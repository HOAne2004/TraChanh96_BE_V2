# --- Giai đoạn 1: Build (Dùng SDK để biên dịch code) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy file dự án (csproj) vào trong container
# Lệnh này copy file từ folder 'drinking-be-v2' ở ngoài vào folder 'drinking-be-v2' bên trong Docker
COPY ["drinking-be-v2/drinking-be-v2.csproj", "drinking-be-v2/"]

# 2. Tải các thư viện cần thiết (Restore)
RUN dotnet restore "drinking-be-v2/drinking-be-v2.csproj"

# 3. Copy toàn bộ source code còn lại vào container
COPY . .

# 4. Chuyển thư mục làm việc vào bên trong folder code để Build
WORKDIR "/src/drinking-be-v2"
# Build bản Release
RUN dotnet build "drinking-be-v2.csproj" -c Release -o /app/build

# --- Giai đoạn 2: Publish (Tạo bộ file chạy) ---
FROM build AS publish
RUN dotnet publish "drinking-be-v2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Giai đoạn 3: Run (Môi trường chạy thực tế - Nhẹ hơn SDK) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy kết quả từ giai đoạn Publish sang đây
COPY --from=publish /app/publish .

# --- CẤU HÌNH CHO RENDER ---
# Render sẽ tự động cấp 1 cổng (PORT) ngẫu nhiên (ví dụ 10000).
# Biến môi trường này bảo .NET lắng nghe đúng cổng đó.
# Nếu không có cổng nào được cấp, nó sẽ chạy mặc định cổng 8080.
ENV ASPNETCORE_HTTP_PORTS=8080
ENV PORT=8080

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "drinking-be-v2.dll"]