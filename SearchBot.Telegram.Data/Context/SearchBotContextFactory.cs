using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SearchBot.Telegram.Data.Context
{
    public class SearchBotContextFactory : IDesignTimeDbContextFactory<SearchBotContext>
    {
        private const string ConnectionStringConst = "User ID=searchUser;Password=searchUserPassword;Host=176.36.160.215;Port=5432;Database=SearchDb;Integrated Security=true;Pooling=true";

        public SearchBotContext  CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseNpgsql(ConnectionStringConst,
                    builder =>
                    {
                        builder.SetPostgresVersion(new Version(14, 1));
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    });

            return new SearchBotContext(optionsBuilder.Options);
        }
    }
}