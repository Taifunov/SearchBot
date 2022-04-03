using SearchBot.Telegram.Api.Utils;
namespace SearchBot.Telegram.Api;

public class BotConfiguration
{ 
    /// <summary>
    /// Gets or sets the Telegram bot token.
    /// </summary>
    public string BotToken { get; init; } = "";

    /// <summary>
    /// Gets or sets the Telegram host address.
    /// </summary>
    public string HostAddress { get; init; } = "";

    /// <summary>
    /// Gets or sets whether to allow any user to
    /// see all registered users.
    /// Defaults to false.
    /// </summary>
    public bool UsersCanSeeAllUsers { get; set; }

    /// <summary>
    /// Loads bot config from TelegramBotConfig.json.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
    /// <returns>
    /// A ValueTuple containing a <see cref="BotConfiguration"/> object and an optional error message.
    /// </returns>
    public static async Task<(BotConfiguration botConfig, string? errMsg)> LoadBotConfigAsync(CancellationToken cancellationToken = default)
    {
        var (botConfig, errMsg) = await FileHelper.LoadJsonAsync<BotConfiguration>("TelegramBotConfig.json", FileHelper.commonJsonDeserializerOptions, cancellationToken);
        if (errMsg is null)
        {
            errMsg = await SaveBotConfigAsync(botConfig, cancellationToken);
        }
        return (botConfig, errMsg);
    }

    /// <summary>
    /// Saves bot config to TelegramBotConfig.json.
    /// </summary>
    /// <param name="botConfig">The <see cref="BotConfig"/> object to save.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the write operation.</param>
    /// <returns>
    /// An optional error message.
    /// Null if no errors occurred.
    /// </returns>
    public static Task<string?> SaveBotConfigAsync(BotConfiguration botConfig, CancellationToken cancellationToken = default)
        => FileHelper.SaveJsonAsync("TelegramBotConfig.json", botConfig, FileHelper.commonJsonSerializerOptions, false, false, cancellationToken);

}
