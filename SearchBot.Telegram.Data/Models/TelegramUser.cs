using System;
using System.Collections.Generic;

namespace SearchBot.Telegram.Data.Models;

public class TelegramUser
{    
    public long Id { get; set; }
    public bool IsBot { get; set; } = false;
    public string FirstName { get; set; } = "";
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? LanguageCode { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastAction { get; set; } = DateTime.UtcNow;
    public bool Banned { get; set; } = false;

    public List<UserMessages> Messages { get; set; } = new();

}