using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Context;
using SearchBot.Telegram.Data.Models;
using TgUser = Telegram.Bot.Types.User;

namespace SearchBot.Telegram.Data.Utils;

public static class DbSetExtensions
{
    public static ValueTask<T?> FindByIdAsync<T>(this DbSet<T> dbSet, object key,
        CancellationToken cancellationToken = default)
        where T : class => dbSet.FindAsync(new[] {key}, cancellationToken);

    public static async Task<TelegramUser> EnsureUserExistAsync(this SearchBotContext context, TgUser? telegramUser, CancellationToken cancellationToken)
    {
        if (telegramUser is null)
        {
            throw new ArgumentException("Message.From is null");
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == telegramUser.Id, cancellationToken);

        if (user is null)
        {
            user = new TelegramUser
            {
                Id = telegramUser.Id,
                Created = DateTime.UtcNow,
                LanguageCode = telegramUser.LanguageCode!.ToUpperInvariant(),
                Username = telegramUser.Username,
                FirstName = telegramUser.FirstName,
                LastName = telegramUser.LastName
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

    public static async Task<TelegramUser> GetUserAsync(this SearchBotContext context, long telegramUserId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == telegramUserId);

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