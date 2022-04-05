using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Context;
using Telegram.Bot;

namespace SearchBot.Telegram.Api.Commands;

public static class GeneralCommand
{
    public static async Task StartAsync(ITelegramBotClient botClient, long chatId, SearchBotContext context, CancellationToken cancellationToken = default)
    {
        const string messageId = "first-message";

        var message = (await context.ReplyMessages.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken))?.Message;

        if (message is null)
        {
            throw new ArgumentNullException($"Message with id '{messageId}' is not exist");
        }

        await botClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }
}