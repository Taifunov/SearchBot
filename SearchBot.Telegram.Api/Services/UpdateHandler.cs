using SearchBot.Telegram.Api.Commands;
using SearchBot.Telegram.Api.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SearchBot.Telegram.Api.Services
{
    public class UpdateHandler
    {
        /// <summary>
        /// Gets public bot commands that are available to all types of chats.
        /// </summary>
        public static BotCommand[] BotCommandsPublic => new BotCommand[]
        {
            new() { Command = "start", Description = "Cleared for takeoff!", }
        };

        /// <summary>
        /// Gets private bot commands that are only available to private chats.
        /// </summary>
        public static BotCommand[] BotCommandsPrivate => new BotCommand[]
        {
            new() { Command = "link", Description = "Link your Telegram account to a user", },
            new() { Command = "unlink", Description = "Unlink your Telegram account from the user", }
        };

        private readonly string _botUsername;
        private readonly BotConfiguration _botConfig;

        public UpdateHandler(string botUsername, BotConfiguration botConfig)
        {
            _botUsername = botUsername;
            _botConfig = botConfig;
        }

        public async Task HandleUpdateStreamAsync(ITelegramBotClient botClient, IAsyncEnumerable<Update> updates, CancellationToken cancellationToken = default)
        {
            await foreach (var update in updates.WithCancellation(cancellationToken))
            {
                try
                {
                    if (update.Type == UpdateType.Message && update.Message is not null)
                    {
                        await HandleCommandAsync(botClient, update.Message, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }

        public Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken = default)
        {
            var (command, argument) = ChatHelper.ParseMessageIntoCommandAndArgument(message.Text, _botUsername);

            // Handle command
            return command switch
            {
                "start" => AuthCommand.StartAsync(botClient, message, _botConfig, cancellationToken),
                "link" => AuthCommand.LinkAsync(botClient, message, argument, cancellationToken),
                "unlink" => AuthCommand.UnlinkAsync(botClient, message, cancellationToken),
                _ => Task.CompletedTask, // unrecognized command, ignoring
            };
        }

        public static void HandleError(Exception ex)
        {
            var errorMessage = ex switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
                _ => ex.ToString(),
            };

            Console.WriteLine(errorMessage);
        }

        public static Task HandleErrorAsync(Exception ex, CancellationToken _ = default)
        {
            HandleError(ex);
            return Task.CompletedTask;
        }
    }
}
