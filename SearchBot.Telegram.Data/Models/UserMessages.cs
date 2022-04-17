using System;

namespace SearchBot.Telegram.Data.Models;

public class UserMessages
{
    public int Id { get; set; }
    public long TelegramUserId { get; set; }
    public long TelegramMessageId { get; set; }
    public string? Username { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public TelegramUser TelegramUser { get; set; } = null!;
}