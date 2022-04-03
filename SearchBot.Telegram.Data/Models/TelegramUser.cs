namespace SearchBot.Telegram.Data.Models;

public class TelegramUser
{
    //
    // Summary:
    //     Unique identifier for this user or bot
    public long Id { get; set; }
    
    //
    // Summary:
    //     True, if this user is a bot
    public bool IsBot { get; set; }
    //
    // Summary:
    //     User's or bot's first name
    public string FirstName { get; set; } = "";
    //
    // Summary:
    //     Optional. User's or bot's last name
    public string? LastName { get; set; }
    //
    // Summary:
    //     Optional. User's or bot's username
    public string? Username { get; set; }
    //
    // Summary:
    //     Optional. IETF language tag of the user's language
    public string? LanguageCode { get; set; }

    
    public User? User { get; set; }

}