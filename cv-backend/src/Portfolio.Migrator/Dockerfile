FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /

COPY ["cv-backend.sln", "."]

COPY ["cv-backend/src/Portfolio.Api/Portfolio.Api.csproj", "cv-backend/src/Portfolio.Api/"]
COPY ["cv-backend/src/Portfolio.Application/Portfolio.Application.csproj", "cv-backend/src/Portfolio.Application/"]
COPY ["cv-backend/src/Portfolio.Domain/Portfolio.Domain.csproj", "cv-backend/src/Portfolio.Domain/"]
COPY ["cv-backend/src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj", "cv-backend/src/Portfolio.Infrastructure/"]
COPY ["cv-backend/src/Portfolio.Migrator/Portfolio.Migrator.csproj", "cv-backend/src/Portfolio.Migrator/"]

RUN ls -la /cv-backend/src/Portfolio.Migrator/

RUN dotnet restore

COPY ["cv-backend/src", "./src"]

RUN dotnet publish src/Portfolio.Migrator -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Portfolio.Migrator.dll"]