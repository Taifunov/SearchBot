using System;

namespace SearchBot.Telegram.Data.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Language { get; set; } = "";
        public DateTime Created { get; set; }
        public DateTime LastAction { get; set; } = DateTime.UtcNow;
        public bool Banned { get; set; }

        public long TelegramUserId { get; set; }
        public TelegramUser? TelegramUser { get; set; }
    }
}