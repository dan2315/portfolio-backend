using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Persistence;

Console.WriteLine("=================================");
Console.WriteLine("  Portfolio Database Migrator");
Console.WriteLine("=================================");
Console.WriteLine();

var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION");

if (string.IsNullOrEmpty(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Error: POSTGRES_CONNECTION environment variable is not set.");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  export POSTGRES_CONNECTION='Host=localhost;Database=portfolio;Username=user;Password=pass'");
    Console.WriteLine("  dotnet run");
    return 1;
}

var dryRun = args.Contains("--dry-run");
var verbose = args.Contains("--verbose") || args.Contains("-v");

if (dryRun)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("🔍 Running in DRY RUN mode - no changes will be applied");
    Console.ResetColor();
    Console.WriteLine();
}

try
{
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseNpgsql(connectionString);
    
    if (verbose)
    {
        optionsBuilder.LogTo(Console.WriteLine);
    }

    await using var context = new AppDbContext(optionsBuilder.Options);

    Console.WriteLine("Connection string: " + connectionString);
    // Check connection
    Console.WriteLine("🔌 Testing database connection...");
    var canConnect = await context.Database.CanConnectAsync();
    
    if (!canConnect)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("❌ Cannot connect to database");
        Console.ResetColor();
        // return 1;
    }
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("✅ Database connection successful");
    Console.ResetColor();
    Console.WriteLine();

    // Get applied migrations
    var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
    Console.WriteLine($"📊 Applied migrations: {appliedMigrations.Count()}");
    
    if (verbose && appliedMigrations.Any())
    {
        foreach (var migration in appliedMigrations)
        {
            Console.WriteLine($"  ✓ {migration}");
        }
        Console.WriteLine();
    }

    // Get pending migrations
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    
    if (!pendingMigrations.Any())
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Database is up to date. No pending migrations.");
        Console.ResetColor();
        return 0;
    }

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"📋 Found {pendingMigrations.Count()} pending migration(s):");
    Console.ResetColor();
    
    foreach (var migration in pendingMigrations)
    {
        Console.WriteLine($"  → {migration}");
    }
    Console.WriteLine();

    if (dryRun)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("🔍 DRY RUN: Migrations would be applied, but skipping due to --dry-run flag");
        Console.ResetColor();
        return 0;
    }

    Console.WriteLine("⚙️  Applying migrations...");
    var startTime = DateTime.UtcNow;
    
    await context.Database.MigrateAsync();
    
    var duration = DateTime.UtcNow - startTime;
    
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✅ Successfully applied {pendingMigrations.Count()} migration(s) in {duration.TotalSeconds:F2} seconds");
    Console.ResetColor();
    
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Migration failed!");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
    
    
    Console.WriteLine();
    Console.WriteLine("Stack trace:");
    Console.WriteLine(ex.StackTrace);
    
    return 1;
}