using Microsoft.EntityFrameworkCore;

using SearchBot.Telegram.Data.Models;

namespace SearchBot.Telegram.Data.Context;

public sealed class SearchBotContext : DbContext
{
    public DbSet<TelegramUser> Users { get; set; }
    public DbSet<UserMessages> Messages { get; set; }
    public DbSet<ReplyMessage> ReplyMessages { get; set; }

#pragma warning disable CS8618
    public SearchBotContext(DbContextOptions options) : base(options) { }
#pragma warning restore CS8618

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TelegramUser>().HasKey(key => key.Id);


        builder.Entity<UserMessages>().Property(it => it.Id).ValueGeneratedOnAdd();
        builder.Entity<UserMessages>().HasKey(it => new { it.TelegramUserId, it.TelegramMessageId });

        builder.Entity<UserMessages>()
            .HasOne(user => user.TelegramUser)
            .WithMany(m => m.Messages)
            .HasForeignKey(user => user.TelegramUserId);

        #region SeedData

        builder.Entity<ReplyMessage>().HasData(
            new ReplyMessage
            {
                Id = "response-message",
                Message =
                    "Спасибо, как только мы просмотрим заявку, сразу же её перешлём в канал, конечно же, если она соответствует правилам публикации. Если заявка не прошла по тем или иным причинам, напишите ваш вопрос сюда."
            },
            new ReplyMessage
            {
                Id = "first-message",
                Message =
                    "Приветствую вас, Призыватель!\r\n\r\nДля начала вам стоит ознакомится с нашими правилами: https://telegra.ph/Pravila-kanala-LoL-Search-07-12\r\n\r\nКаждая заявка должна быть сформулирована по следующим пунктам и описанию:\r\n\r\nОбразец:\r\nТег сервера: #RU (его нужно указать над заявкой, без него пост не пройдёт)\r\n\r\n1) Ваш ранг и сервер (если нет ранга укажите хотя бы ваш уровень)\r\n2) Ваша роль \r\n3) Ваше предпочитаемое время игры (пример: 14-18 по МСК)\r\n4) Удобное средство связи\r\n5) Немного о себе; Кого ищешь.  (кратко)\r\n\r\nПри несоблюдении данного шаблона - запись не публикуется."
            },
            new ReplyMessage { Id = "banned-user", Message = "Вы забанены на нашем сервере. Попытки написать нам более не имеют успеха. Желаем всего хорошего." });
        #endregion
    }
}