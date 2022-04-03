using Telegram.Bot.Types;

namespace SearchBot.Telegram.Api.Services;

public interface IHandleUpdateService
{
    Task HandleUpdate(Update update);
}