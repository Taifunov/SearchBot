using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Context;
using SearchBot.Telegram.Data.Models;
using Telegram.Bot.Types;
using User = SearchBot.Telegram.Data.Models.User;

namespace SearchBot.Telegram.Data.Utils;

public static class DbSetExtensions
{
    public static ValueTask<T?> FindByIdAsync<T>(this DbSet<T> dbSet, object key,
        CancellationToken cancellationToken = default)
        where T : class => dbSet.FindAsync(new[] {key}, cancellationToken);

    public static async Task<User> EnsureUserExistAsync(this SearchBotContext context, Message message, CancellationToken cancellationToken)
    {
        if (message.From is null)
        {
            throw new ArgumentException("Message.From is null");
        }

        var user = await context.Users.Include(tu => tu.TelegramUser).
            FirstOrDefaultAsync(u => u.TelegramUser!.Id == message.From.Id, cancellationToken);

        if (user is null)
        {
            user = new User
            {
                TelegramUserId = message.From.Id,
                Created = DateTime.UtcNow,
                Language = message.From.LanguageCode!.ToUpperInvariant(),
                TelegramUser = new TelegramUser
                {
                    Id = message.From.Id,
                    Username = message.From.Username,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                    IsBot = message.From.IsBot,
                    LanguageCode = message.From.LanguageCode
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

    public static void TryRemove<T>(this DbContext context, T? entity)
    {
        if (entity is not null)
        {
            context.Remove(entity);
        }
    }
}