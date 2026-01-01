@echo off
setlocal enabledelayedexpansion

echo ====================================
echo   Portfolio Database Migrator
echo ====================================
echo.

REM Load .env if POSTGRES_CONNECTION is not set
if "%POSTGRES_CONNECTION%"=="" (
    if exist ".env" (
        for /f "usebackq tokens=1* delims==" %%A in (".env") do (
            set "%%A=%%B"
        )
    )
)

REM Check if POSTGRES_CONNECTION is set
if "%POSTGRES_CONNECTION%"=="" (
    echo [ERROR] POSTGRES_CONNECTION environment variable is not set.
    echo.
    echo Please set it first:
    echo   set POSTGRES_CONNECTION=Host=localhost;Database=portfolio;Username=postgres;Password=yourpass
    echo.
    echo Or create a .env file with the connection string.
    echo.
    pause
    exit /b 1
)

REM Parse command line arguments
set DRY_RUN=
set VERBOSE=
set REBUILD=

:parse_args
if "%~1"=="" goto end_parse
if /i "%~1"=="--dry-run" set DRY_RUN=--dry-run
if /i "%~1"=="--verbose" set VERBOSE=--verbose
if /i "%~1"=="-v" set VERBOSE=--verbose
if /i "%~1"=="--rebuild" set REBUILD=1
shift
goto parse_args
:end_parse

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not running. Please start Docker Desktop.
    echo.
    pause
    exit /b 1
)

REM Build the image if needed
docker images portfolio-migrator -q | find /v "" >nul
if errorlevel 1 set REBUILD=1

if defined REBUILD (
    echo [INFO] Building migrator image...
    echo.
    docker build -t portfolio-migrator -f ./cv-backend/src/Portfolio.Migrator/Dockerfile .
    if errorlevel 1 (
        echo.
        echo [ERROR] Failed to build Docker image.
        pause
        exit /b 1
    )
    echo.
    echo [SUCCESS] Image built successfully.
    echo.
) else (
    echo [INFO] Using existing image (use --rebuild to rebuild)
    echo.
)

REM Show configuration
echo [CONFIG] Connection string configured
if defined DRY_RUN echo [CONFIG] Mode: DRY RUN
if defined VERBOSE echo [CONFIG] Verbose: ON
echo.

REM Run the migrator
echo [INFO] Running migrations...
echo.

docker run --rm ^
  -e POSTGRES_CONNECTION="%POSTGRES_CONNECTION%" ^
  portfolio-migrator %DRY_RUN% %VERBOSE%