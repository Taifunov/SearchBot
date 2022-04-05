using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Context;
using SearchBot.Telegram.Data.Models;
using User = SearchBot.Telegram.Data.Models.User;
using TgUser = Telegram.Bot.Types.User;

namespace SearchBot.Telegram.Data.Utils;

public static class DbSetExtensions
{
    public static ValueTask<T?> FindByIdAsync<T>(this DbSet<T> dbSet, object key,
        CancellationToken cancellationToken = default)
        where T : class => dbSet.FindAsync(new[] {key}, cancellationToken);

    public static async Task<User> EnsureUserExistAsync(this SearchBotContext context, TgUser? telegramUser, CancellationToken cancellationToken)
    {
        if (telegramUser is null)
        {
            throw new ArgumentException("Message.From is null");
        }

        var user = await context.Users.Include(tu => tu.TelegramUser).
            FirstOrDefaultAsync(u => u.TelegramUser!.Id == telegramUser.Id, cancellationToken);

        if (user is null)
        {
            user = new User
            {
                TelegramUserId = telegramUser.Id,
                Created = DateTime.UtcNow,
                Language = telegramUser.LanguageCode!.ToUpperInvariant(),
                TelegramUser = new TelegramUser
                {
                    Id = telegramUser.Id,
                    Username = telegramUser.Username,
                    FirstName = telegramUser.FirstName,
                    LastName = telegramUser.LastName,
                    IsBot = telegramUser.IsBot,
                    LanguageCode = telegramUser.LanguageCode
                }
            };

            await context.AddAsync(user, cancellationToken);
        }
        else
        {
            user.LastAction = DateTime.UtcNow;
            context.Update(user);
        }

        await context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public static async Task<User> GetUserAsync(this SearchBotContext context, long telegramUserId)
    {
        var user = await context.Users.Include(tg => tg.TelegramUser).FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);

        if (user is null)
        {
            throw new ArgumentNullException($"User with id '{telegramUserId}' is not exist");
        }

        return user;
    }

    public static void TryRemove<T>(this DbContext context, T? entity)
    {
        if (entity is not null)
        {
            context.Remove(entity);
        }
    }
}