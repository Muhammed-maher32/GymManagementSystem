using GymSystem.DAL.Data.AppDbContexts;
using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GymManagementSystem.DAL.Data.DataSeed;

public static class GymDataSeeding
{
    public static async Task SeedAsync(GymDbContext dbContext, string seedFilesPath, ILogger logger, CancellationToken ct = default)
    {
        try
        {
            if (!await dbContext.Plans.AnyAsync(ct))
            {
                var plans = LoadDataFromJsonFile<Plan>("plans.json", seedFilesPath);
                if (plans.Count > 0)
                {
                    dbContext.Plans.AddRange(plans);
                    logger.LogInformation("Seeded {Count} plans.", plans.Count);
                }
            }

            if (dbContext.ChangeTracker.HasChanges())
                await dbContext.SaveChangesAsync(ct);
            else
                logger.LogInformation("Plan Already Seeded");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gym data seeding failed.");
            throw;
        }
    }

    private static List<T> LoadDataFromJsonFile<T>(string fileName, string FolderPath)
    {

        var filePath = Path.Combine(FolderPath, fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed data file not found: {filePath}");

        var data = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
    }
}
