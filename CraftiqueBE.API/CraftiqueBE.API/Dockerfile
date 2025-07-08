# Sử dụng image ASP.NET Core runtime làm base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Sử dụng SDK image để build dự án
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CraftiqueBE.API.csproj", "."]
COPY ["CraftiqueBE.Data/CraftiqueBE.Data.csproj", "CraftiqueBE.Data/"]
COPY ["CraftiqueBE.Service/CraftiqueBE.Service.csproj", "CraftiqueBE.Service/"]
RUN dotnet restore "CraftiqueBE.API.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "CraftiqueBE.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CraftiqueBE.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CraftiqueBE.API.dll"]