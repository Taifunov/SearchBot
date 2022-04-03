using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SearchBot.Telegram.Api.Commands;

public static class AuthCommand
{
    //private readonly ILogger<AuthCommand> _logger;
    //public AuthCommand(ILogger<AuthCommand> logger)
    //{
    //    _logger = logger;
    //}

    public static Task StartAsync(ITelegramBotClient botClient, Message message, BotConfiguration config, CancellationToken cancellationToken = default)
    {
        //_logger.LogInformation($"{message.From} executed {message.Text} in {message.Chat.Type.ToString().ToLower()} chat {(string.IsNullOrEmpty(message.Chat.Title) ? string.Empty : $"{message.Chat.Title} ")}({message.Chat.Id}).");


        var replyMarkdownV2 = $@"🧑‍✈️ Good evening\! Thank you for flying with us\. ✈️ To get your boarding pass, please use `/link <UUID>` to link your Telegram account to your user\.";

        return botClient.SendTextMessageAsync(message.Chat.Id,
            replyMarkdownV2,
            ParseMode.MarkdownV2,
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }

    public static async Task LinkAsync(ITelegramBotClient botClient, Message message, string? argument, CancellationToken cancellationToken = default)
    {
        Console.Write($"{message.From} executed {message.Text} in {message.Chat.Type.ToString().ToLower()} chat {(string.IsNullOrEmpty(message.Chat.Title) ? string.Empty : $"{message.Chat.Title} ")}({message.Chat.Id}).");

        string reply = string.Empty;

        var (botConfig, loadBotConfigErrMsg) = await BotConfiguration.LoadBotConfigAsync(cancellationToken);
        if (loadBotConfigErrMsg is not null)
        {
            Console.WriteLine(loadBotConfigErrMsg);
            return;
        }


        await botClient.SendTextMessageAsync(message.Chat.Id,
                                             reply,
                                             replyToMessageId: message.MessageId,
                                             cancellationToken: cancellationToken);
    }

    public static async Task UnlinkAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken = default)
    {
        //_logger.LogInformation($"{message.From} executed {message.Text} in {message.Chat.Type.ToString().ToLower()} chat {(string.IsNullOrEmpty(message.Chat.Title) ? string.Empty : $"{message.Chat.Title} ")}({message.Chat.Id}).");

        var (botConfig, loadBotConfigErrMsg) = await BotConfiguration.LoadBotConfigAsync(cancellationToken);
        if (loadBotConfigErrMsg is not null)
        {
            Console.WriteLine(loadBotConfigErrMsg);
            return;
        }

        var reply = "You are not linked to any user.";
        Console.WriteLine("Response: not linked");
       //_logger.LogInformation(" Response: not linked");

        await botClient.SendTextMessageAsync(message.Chat.Id,
                                         reply,
                                         replyToMessageId: message.MessageId,
                                         cancellationToken: cancellationToken);
    }
}