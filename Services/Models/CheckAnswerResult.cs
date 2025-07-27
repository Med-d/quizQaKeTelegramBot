using System;

namespace quizQaKeTelegramBot.Services.Models;

public record CheckAnswerResult
{
    public required bool IsAnswerCorrect { get; set; }
    public required int RemainingAttempts { get; set; }
}
