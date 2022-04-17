using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Context;
using SearchBot.Telegram.Data.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SearchBot.Telegram.Api.Commands;

public static class ModerationCommand
{
    public static async Task BanAsync(ITelegramBotClient botClient, SearchBotContext context, Message message, string? argument, CancellationToken cancellationToken)
    {
        var replyMessage = message.ReplyToMessage;

        if (replyMessage is null)
        {
            throw new ArgumentNullException("Reply message is null");
        }

        var userToBan = await context.GetUserAsync(replyMessage.ForwardFrom!.Id);

        if (userToBan.Banned)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"{replyMessage.ForwardFrom.FirstName} is already banned.", cancellationToken: cancellationToken);
            return;
        }

        userToBan.Banned = true;
        context.Update(userToBan);
        await context.SaveChangesAsync(cancellationToken);

        var replyBannedUserMsg = await context.ReplyMessages.FirstOrDefaultAsync(x => x.Id == "banned-user", cancellationToken);

        var replyMessageToBannedUser = replyBannedUserMsg != null ? replyBannedUserMsg.Message + $"Причина: {(!string.IsNullOrEmpty(argument) ? argument : "не указана.")}" : $"Вы были заблокированны администратором. Причина: {(!string.IsNullOrEmpty(argument) ? argument : "не указана.")}";

        await botClient.SendTextMessageAsync(replyMessage.ForwardFrom.Id, replyMessageToBannedUser, cancellationToken: cancellationToken);
        await botClient.SendTextMessageAsync(message.Chat.Id, $"{replyMessage.ForwardFrom.FirstName} banned forever.", cancellationToken: cancellationToken);
    }

    public static async Task UnBanAsync(ITelegramBotClient botClient, SearchBotContext context, Message message, string? argument, CancellationToken cancellationToken)
    {
        var replyMessage = message.ReplyToMessage;

        if (replyMessage is null)
        {
            throw new ArgumentNullException("Reply message is null");
        }

        var userToUnban = await context.GetUserAsync(replyMessage.ForwardFrom!.Id);

        userToUnban.Banned = false;
        context.Update(userToUnban);
        await context.SaveChangesAsync(cancellationToken);

        await botClient.SendTextMessageAsync(message.Chat.Id, $"{replyMessage.ForwardFrom.FirstName} was unbanned.", cancellationToken: cancellationToken);
    }
}