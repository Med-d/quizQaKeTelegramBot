using System;
using Microsoft.EntityFrameworkCore;

namespace quizQaKeTelegramBot.DataBase.Models;

[PrimaryKey(nameof(Id))]
public class Question
{
    public Guid Id { get; set; }
    public required string Fact { get; set; }
    public required string Hobby { get; set; }
    
    public required string Answer { get; set; }

    public required string FirstClue { get; set; }
    public required string SecondClue { get; set; }
}
