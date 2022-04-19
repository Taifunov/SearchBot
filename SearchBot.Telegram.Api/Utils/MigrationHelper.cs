using Microsoft.EntityFrameworkCore;

namespace SearchBot.Telegram.Api.Utils;

public static class MigrationHelper
{
    public static async Task MigrateDatabase<T>(this IServiceProvider services) where T : DbContext
    {
        var logger = services.GetRequiredService<ILogger<Startup>>();

        try
        {
            var db = services.GetRequiredService<T>();

            var isMigrationNeeded = (await db.Database.GetPendingMigrationsAsync()).Any();
            if (isMigrationNeeded)
            {
                await db.Database.MigrateAsync();
                logger.LogInformation("Database Migrated Successfully");
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}