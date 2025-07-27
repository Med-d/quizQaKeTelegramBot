namespace quizQaKeTelegramBot.Services.Models;

public record class UserScore
{
    public required string TelegramLogin { get; set; }
    public required int Score { get; set; }
    public required DateTime LastAnswerDateTime { get; set; }
}
