using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Context;
using SearchBot.Telegram.Data.Models;
using SearchBot.Telegram.Data.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = SearchBot.Telegram.Data.Models.User;

namespace SearchBot.Telegram.Api.Services;

public class HandleUpdateService : IHandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly SearchBotContext _context;
    private readonly long _adminId;

    public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger, SearchBotContext context)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
#if RELEASE
        _adminId = long.Parse(Environment.GetEnvironmentVariable("AdminId"));
#else
        _adminId = 163842273;
#endif
    }

    public async Task HandleUpdate(Update update)
    {
        var cts = new CancellationTokenSource();

        var handler = update.Type switch
        {
            UpdateType.Message => HandleMessage(update.Message!, cts.Token),
            _ => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
            _logger.LogError(exception, exception.InnerException?.ToString());
        }
    }

    public async Task HandleMessage(Message message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Receive message type: {messageType}", message.Type);

        if (message.Type != MessageType.Text || message.Text is null)
            return;

        if (message.Chat.Type == ChatType.Private)
        {
            if (message.ReplyToMessage is not null)
            {
                await ReplyToUserMessageAsync(message, cancellationToken);
                return;
            }

            if (message.Text!.Split(' ')[0] == "/start")
            {
                await SendResponseMessageAsync("first-message", message.Chat.Id, cancellationToken);
                return;
            }

            var user = await _context.EnsureUserExistAsync(message, cancellationToken);

            await SaveMessageAsync(user, message.Text, message.MessageId,cancellationToken);

            await _botClient.ForwardMessageAsync(_adminId, message.Chat.Id, message.MessageId, cancellationToken: cancellationToken);
            
            if (user.LastAction.AddHours(1) > DateTime.UtcNow)
            {
                await SendResponseMessageAsync("response-message", message.Chat.Id, cancellationToken);
            }
        }
    }

    private async Task SendResponseMessageAsync(string messageId, long chatId, CancellationToken cancellationToken = default)
    {
        var message = (await _context.ReplyMessages.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken))?.Message;

        if (message is null)
        {
            throw new ArgumentNullException($"{messageId} is not exist");
        }

        await _botClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }

    private async Task SaveMessageAsync(User user, string message, int messageId, CancellationToken cancellationToken = default)
    {
        if (user.TelegramUser is null)
        {
            throw new ArgumentNullException("Telegram user is null");
        }

        var messageToSave = new UserMessages
        {
            Username = user.TelegramUser!.Username!,
            TelegramUserId = user.TelegramUser.Id,
            TelegramMessageId = messageId,
            Content = message
        };

        await _context.AddAsync(messageToSave, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task ReplyToUserMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        var replyMessage = message.ReplyToMessage;

        var messageToReply = await _context.Messages.FirstOrDefaultAsync(x => x.TelegramUserId == replyMessage!.ForwardFrom!.Id && x.Content == replyMessage.Text, cancellationToken);

        if (replyMessage is null)
        {
            throw new ArgumentNullException("Reply message is null");
        }

        await _botClient.SendTextMessageAsync(replyMessage.ForwardFrom!.Id, message.Text!, replyToMessageId: messageToReply?.TelegramMessageId, cancellationToken: cancellationToken);
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {updateType}", update.Type);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
}
