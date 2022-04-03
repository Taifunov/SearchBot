using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Models;
using User = SearchBot.Telegram.Data.Models.User;
//using TelegramUser = Telegram.Bot.Types.User;

namespace SearchBot.Telegram.Data.Context;


public sealed class SearchBotContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<TelegramUser> TelegramUsers { get; set; }
    public DbSet<UserMessages> Messages { get; set; }
    public DbSet<ReplyMessage> ReplyMessages { get; set; }

    public SearchBotContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        //builder.Entity<User>().HasKey(prop => prop.Id);

        //builder.Entity<TelegramUser>().HasKey(prop => prop.Id);
        //builder.Entity<User>()
        //    .HasOne(user => user.TelegramUser)
        //    .WithOne(tgUser => tgUser.User)
        //    .HasForeignKey<TelegramUser>(user => user.Id);


        //builder.Entity<UserMessages>().HasKey(key => key.Id);
        //builder.Entity<UserMessages>().Property(prop => prop.Id)
        //    .ValueGeneratedOnAdd();
    }
}
