FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /

COPY ["cv-backend.sln", "."]

COPY ["cv-backend/src/Portfolio.Domain/Portfolio.Domain.csproj", "cv-backend/src/Portfolio.Domain/"]
COPY ["cv-backend/src/Portfolio.Application/Portfolio.Application.csproj", "cv-backend/src/Portfolio.Application/"]
COPY ["cv-backend/src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj", "cv-backend/src/Portfolio.Infrastructure/"]
COPY ["cv-backend/src/Portfolio.Migrator/Portfolio.Migrator.csproj", "cv-backend/src/Portfolio.Migrator/"]

RUN dotnet restore cv-backend/src/Portfolio.Migrator/Portfolio.Migrator.csproj 

COPY ["cv-backend/src/Portfolio.Domain", "cv-backend/src/Portfolio.Domain/"]
COPY ["cv-backend/src/Portfolio.Application", "cv-backend/src/Portfolio.Application/"]
COPY ["cv-backend/src/Portfolio.Infrastructure", "cv-backend/src/Portfolio.Infrastructure/"]
COPY ["cv-backend/src/Portfolio.Migrator", "cv-backend/src/Portfolio.Migrator/"]

RUN dotnet publish cv-backend/src/Portfolio.Migrator -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Portfolio.Migrator.dll"]