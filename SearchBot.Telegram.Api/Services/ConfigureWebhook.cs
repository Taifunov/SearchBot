using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SearchBot.Telegram.Api.Services;

public class ConfigureWebhook : IHostedService
{
    private readonly ILogger<ConfigureWebhook> _logger;
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _botConfig;
    private readonly string _botToken;
    private readonly string _hostAddress;

    public ConfigureWebhook(ILogger<ConfigureWebhook> logger,
                            IServiceProvider serviceProvider,
                            IConfiguration configuration)
    {
        _logger = logger;
        _services = serviceProvider;
        _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
#if RELEASE
        _hostAddress = Environment.GetEnvironmentVariable("HostAddress") ?? throw new ArgumentNullException($"{nameof(_hostAddress)} is null");
        _botToken = Environment.GetEnvironmentVariable("BotToken") ?? throw new ArgumentNullException($"{nameof(_botToken)} is null");
#else
        _botToken = _botConfig.BotToken;
        _hostAddress = _botConfig.HostAddress;
#endif
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        var me = await botClient.GetMeAsync(cancellationToken);

        var (botConfig, loadBotConfigErrMsg) = await BotConfiguration.LoadBotConfigAsync(cancellationToken);

        var webhookAddress = @$"{_hostAddress}/bot/{_botToken}";
        _logger.LogInformation("Setting webhook: {webhookAddress}", webhookAddress);
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        // Remove webhook upon app shutdown
        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}
