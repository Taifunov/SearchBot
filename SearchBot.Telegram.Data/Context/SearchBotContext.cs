﻿using Microsoft.EntityFrameworkCore;
using SearchBot.Telegram.Data.Models;
using User = SearchBot.Telegram.Data.Models.User;

namespace SearchBot.Telegram.Data.Context;

public sealed class SearchBotContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<TelegramUser> TelegramUsers { get; set; }
    public DbSet<UserMessages> Messages { get; set; }
    public DbSet<ReplyMessage> ReplyMessages { get; set; }

#pragma warning disable CS8618
    public SearchBotContext(DbContextOptions options) : base(options) { }
#pragma warning restore CS8618
}
