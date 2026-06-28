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
            await SeedPlansAsync(dbContext, seedFilesPath, logger, ct);
            await SeedTrainersAsync(dbContext, seedFilesPath, logger, ct);

            if (dbContext.ChangeTracker.HasChanges())
            {
                await dbContext.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gym data seeding failed.");
            throw;
        }
    }

    private static async Task SeedPlansAsync(GymDbContext dbContext, string seedFilesPath, ILogger logger, CancellationToken ct)
    {
        if (await dbContext.Plans.AnyAsync(ct))
        {
            logger.LogInformation("Plans already seeded.");
            return;
        }

        var plans = LoadDataFromJsonFile<Plan>("plans.json", seedFilesPath);

        if (plans.Count == 0)
            return;

        dbContext.Plans.AddRange(plans);

        logger.LogInformation("Seeded {Count} plans.", plans.Count);
    }

    private static async Task SeedTrainersAsync(GymDbContext dbContext, string seedFilesPath, ILogger logger, CancellationToken ct)
    {
        if (await dbContext.Trainers.AnyAsync(ct))
        {
            logger.LogInformation("Trainers already seeded.");
            return;
        }

        var trainers = LoadDataFromJsonFile<Trainer>("trainers.json", seedFilesPath);

        if (trainers.Count == 0)
            return;

        dbContext.Trainers.AddRange(trainers);

        logger.LogInformation("Seeded {Count} trainers.", trainers.Count);
    }

    private static List<T> LoadDataFromJsonFile<T>(
        string fileName,
        string folderPath)
    {
        var filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed data file not found: {filePath}");

        var data = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
    }
}
