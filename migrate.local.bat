@echo off
echo ====================================
echo   Portfolio Database Migrator
echo   (Local Development)
echo ====================================
echo.

REM Set local connection string
set POSTGRES_CONNECTION=Host=localhost;Database=portfolio;Username=postgres;Password=postgres

echo [INFO] Using local database connection
echo.

REM Run without Docker
dotnet run -- %*

if errorlevel 1 (
    echo.
    echo [ERROR] Migration failed!
    pause
    exit /b 1
)

echo.
echo [SUCCESS] Migration completed!
echo.
pause
```

## Создай .env.example для документации:

**src/Portfolio.Migrator/.env.example**
```
POSTGRES_CONNECTION=Host=localhost;Database=portfolio;Username=postgres;Password=yourpassword