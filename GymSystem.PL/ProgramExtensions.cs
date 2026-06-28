using GymManagementSystem.DAL.Data.DataSeed;
using GymSystem.DAL.Data.AppDbContexts;
using GymSystemG04;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.PL;

public static class ProgramExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var pending = await dbContext.Database.GetPendingMigrationsAsync();
        if (pending.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations...", pending.Count());
            await dbContext.Database.MigrateAsync();
        }

        var seedPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
        await GymDataSeeding.SeedAsync(dbContext, seedPath, logger);
    }
}
