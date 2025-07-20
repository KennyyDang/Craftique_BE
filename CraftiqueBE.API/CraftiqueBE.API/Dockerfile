# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CraftiqueBE.API/CraftiqueBE.API.csproj", "CraftiqueBE.API/"]
COPY ["CraftiqueBE.Service/CraftiqueBE.Service.csproj", "CraftiqueBE.Service/"]
COPY ["CraftiqueBE.Data/CraftiqueBE.Data.csproj", "CraftiqueBE.Data/"]
RUN dotnet restore "./CraftiqueBE.API/CraftiqueBE.API.csproj"
COPY . .
WORKDIR "/src/CraftiqueBE.API"
RUN dotnet build "./CraftiqueBE.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CraftiqueBE.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Tạo user non-root để security tốt hơn
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Tạo thư mục uploads và set quyền
RUN mkdir -p /app/wwwroot/uploads/custom-products && \
    mkdir -p /app/wwwroot/uploads/designs && \
    chown -R appuser:appuser /app/wwwroot && \
    chmod -R 755 /app/wwwroot/uploads

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

# Chuyển sang user appuser
USER appuser

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "CraftiqueBE.API.dll"]