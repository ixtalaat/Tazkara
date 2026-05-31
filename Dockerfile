# Stage 1: Build Image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Tazkara.Shared/Tazkara.Shared.csproj", "Tazkara.Shared/"]
COPY ["Tazkara.Domain/Tazkara.Domain.csproj", "Tazkara.Domain/"]
COPY ["Tazkara.Application/Tazkara.Application.csproj", "Tazkara.Application/"]
COPY ["Tazkara.Infrastructure/Tazkara.Infrastructure.csproj", "Tazkara.Infrastructure/"]
COPY ["Tazkara.API/Tazkara.API.csproj", "Tazkara.API/"]
RUN dotnet restore "Tazkara.API/Tazkara.API.csproj"

# Copy remaining source code
COPY . .
WORKDIR "/src/Tazkara.API"
RUN dotnet build "Tazkara.API.csproj" -c Release -o /app/build

# Stage 2: Publish Image
FROM build AS publish
RUN dotnet publish "Tazkara.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Run as non-root user
USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tazkara.API.dll"]
