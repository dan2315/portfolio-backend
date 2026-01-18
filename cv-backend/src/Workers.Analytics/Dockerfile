FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /

COPY ["cv-backend.sln", "."]

COPY ["cv-backend/src/Workers.Analytics/Workers.Analytics.csproj", "cv-backend/src/Workers.Analytics/"]
COPY ["cv-backend/src/Portfolio.Application/Portfolio.Application.csproj", "cv-backend/src/Portfolio.Application/"]
COPY ["cv-backend/src/Portfolio.Domain/Portfolio.Domain.csproj", "cv-backend/src/Portfolio.Domain/"]
COPY ["cv-backend/src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj", "cv-backend/src/Portfolio.Infrastructure/"]

RUN dotnet restore cv-backend/src/Workers.Analytics/Workers.Analytics.csproj

COPY ["cv-backend/src/Workers.Analytics", "cv-backend/src/Workers.Analytics/"]
COPY ["cv-backend/src/Portfolio.Application", "cv-backend/src/Portfolio.Application/"]
COPY ["cv-backend/src/Portfolio.Domain", "cv-backend/src/Portfolio.Domain/"]
COPY ["cv-backend/src/Portfolio.Infrastructure", "cv-backend/src/Portfolio.Infrastructure/"]

RUN dotnet publish cv-backend/src/Workers.Analytics -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build ["/app/publish", "."]

EXPOSE 8080
ENTRYPOINT ["dotnet", "Workers.Analytics.dll"]