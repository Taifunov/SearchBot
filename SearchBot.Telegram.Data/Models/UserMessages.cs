﻿using System;

namespace SearchBot.Telegram.Data.Models;

public class UserMessages
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.UtcNow;
}