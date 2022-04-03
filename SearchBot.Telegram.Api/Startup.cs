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
        Token = BotConfig.BotToken;
    }

    public IConfiguration Configuration { get; }
    private BotConfiguration BotConfig { get; }
    private string Token { get; }

    private const string ConnectionStringConst = "User ID=searchUser;Password=searchUserPassword;Host=176.36.160.215;Port=5432;Database=SearchDb;Integrated Security=true;Pooling=true";


    public void ConfigureServices(IServiceCollection services)
    {
        // There are several strategies for completing asynchronous tasks during startup.
        // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
        // We are going to use IHostedService to add and later remove Webhook
        services.AddHostedService<ConfigureWebhook>();

        // Register named HttpClient to get benefits of IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(Token, httpClient));

        // Dummy business-logic service
        services.AddDbContextFactory<SearchBotContext>((provider, builder) =>
            builder.UseNpgsql(ConnectionStringConst, o => SetupNpgSqlDbOpts(o))
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(MyLoggerFactory));

        services.AddTransient<IHandleUpdateService, HandleUpdateService>();

        //services.AddScoped<IHandleUpdateService, HandleUpdateService>();
        // The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
        // incoming webhook updates and send serialized responses back.
        // Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
        //   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0#add-newtonsoftjson-based-json-format-support
        services.AddControllers()
                .AddNewtonsoftJson();
    }

    public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .AddConsole()
            .AddFilter((category, level)
                => category == DbLoggerCategory.Database.Command.Name
                   && level == LogLevel.Information);
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
            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            var token = BotConfig.BotToken;
            endpoints.MapControllerRoute("webhook", $"bot/{Token}", new { controller = "Update", action = "Post" });
            endpoints.MapControllerRoute("webhook-get", $"bot/{Token}", new { controller = "Update", action = "Get" });
            endpoints.MapControllers();
        });
    }
}
