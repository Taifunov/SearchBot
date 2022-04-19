namespace SearchBot.Telegram.Api.Dtos;

public class UserLastMessageDto
{
    public long TelegramUserId { get; set; }
    public string? Firstname { get; set; }
    public string? LastName { get; set; }
    public string? Message { get; set; }
    public DateTime LastAction { get; set; }

    public override string ToString()
    {
        return $"--------------------\nName: {Firstname}\nMessage: {Message}\nLastAction: {LastAction.ToLocalTime()}";
    }
}