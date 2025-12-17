# --- Giai đoạn 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy file csproj vào folder con trong container
# Cấu trúc: COPY [Đường_dẫn_trên_máy_bạn] [Đường_dẫn_trong_container]
COPY ["drinking-be-v2/drinking-be-v2.csproj", "drinking-be-v2/"]

# 2. Restore các thư viện (nuget packages)
RUN dotnet restore "drinking-be-v2/drinking-be-v2.csproj"

# 3. Copy toàn bộ source code còn lại
COPY . .

# 4. Chuyển hướng vào thư mục chứa code chính để build
WORKDIR "/src/drinking-be-v2"
RUN dotnet build "drinking-be-v2.csproj" -c Release -o /app/build

# --- Giai đoạn 2: Publish ---
FROM build AS publish
RUN dotnet publish "drinking-be-v2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Giai đoạn 3: Run (Final) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Cấu hình Port (Railway thường tự override cái này, nhưng cứ để cho chắc)
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "drinking-be-v2.dll"]