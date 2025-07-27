using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace quizQaKeTelegramBot.DataBase.Models;

[PrimaryKey(nameof(UserId), nameof(QuestionId))]
public class Answer
{
    [ForeignKey(nameof(User.Id))]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(Question.Id))]
    public Guid QuestionId { get; set; }

    public bool UsedFirstClue { get; set; }
    public bool UsedSecondClue { get; set; }

    public int Attempt { get; set; }

    public AnswerState State { get; set; }
    public DateTime? AnsweredDateTime { get; set; }
    
    public string[] AlternativeAnswers { get; set; }
}
