using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using SearchBot.Telegram.Api.Services;
using SearchBot.Telegram.Data.Context;
using Telegram.Bot;

namespace SearchBot.Telegram.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

#if RELEASE
        Token = Environment.GetEnvironmentVariable("BotToken") ?? throw new InvalidOperationException($"{nameof(Token)} is empty"); ;
        _connectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? throw new InvalidOperationException("ConnectionString is empty");
#else
        Token = BotConfig.BotToken;
        _connectionString = Configuration.GetConnectionString(nameof(SearchBotContext));
#endif
    }

    public IConfiguration Configuration { get; }
    private BotConfiguration BotConfig { get; }
    private string Token { get; }
    private readonly string _connectionString;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<ConfigureWebhook>();

        services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(Token, httpClient));

        
        services.AddDbContextFactory<SearchBotContext>((provider, builder) =>
            builder.UseNpgsql(_connectionString, o => SetupNpgSqlDbOpts(o))
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(MyLoggerFactory));

        services.AddTransient<IHandleUpdateService, HandleUpdateService>();
        services.AddControllers()
                .AddNewtonsoftJson();
    }

    public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .AddConsole()
            .AddFilter((category, level)
                => category == DbLoggerCategory.Database.Command.Name
                   && level == LogLevel.Warning);
    });

    public static NpgsqlDbContextOptionsBuilder SetupNpgSqlDbOpts(NpgsqlDbContextOptionsBuilder opts)
    {
        opts.SetPostgresVersion(new Version(14, 1));
        opts.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
        return opts;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseRouting();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("webhook", $"bot/{Token}", new { controller = "Update", action = "Post" });
            endpoints.MapControllerRoute("webhook-get", $"bot/{Token}", new { controller = "Update", action = "Get" });
            endpoints.MapControllers();
        });
    }
}
