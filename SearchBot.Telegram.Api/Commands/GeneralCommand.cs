using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Api.Dtos;
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

    public static async Task GetLast3DaysMessagesAsync(ITelegramBotClient botClient, long tgUserId, SearchBotContext context, long sendToId, CancellationToken cancellationToken = default)
    {
        var messages = await context.Messages
            .Include(t => t.TelegramUser)
            .Where(m => m.TelegramUserId == tgUserId && m.TelegramUser.LastAction <= DateTime.UtcNow &&
                        m.TelegramUser.LastAction >= DateTime.UtcNow.AddDays(-3))
            .Select(m => new UserLastMessageDto
            {
                TelegramUserId = m.TelegramUserId,
                Firstname = m.TelegramUser.FirstName,
                Message = m.Content,
                LastAction = m.TelegramUser.LastAction
            })
            .ToListAsync(cancellationToken);

        var usersMsg = string.Join("\n", messages.Select(m => m.ToString()));

        var messageToSend = string.IsNullOrEmpty(usersMsg) ? "Нет сообщений за данный период" : usersMsg;

        await botClient.SendTextMessageAsync(sendToId, messageToSend, cancellationToken: cancellationToken);
    }
}