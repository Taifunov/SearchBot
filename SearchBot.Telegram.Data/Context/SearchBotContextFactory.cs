using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SearchBot.Telegram.Data.Context
{
    public class SearchBotContextFactory : IDesignTimeDbContextFactory<SearchBotContext>
    {
        public SearchBotContext  CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
#if RELEASE
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? throw new InvalidOperationException("ConnectionString is empty");
#else
            //var connectionString = configuration.GetConnectionString(nameof(SearchBotContext)) ?? throw new InvalidOperationException($"ConnectionString '{nameof(SearchBotContext)}' is empty");
#endif
            var optionsBuilder = new DbContextOptionsBuilder()
                .UseNpgsql(connectionString,
                    builder =>
                    {
                        builder.SetPostgresVersion(new Version(14, 1));
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    })
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            return new SearchBotContext(optionsBuilder.Options);
        }
    }
}