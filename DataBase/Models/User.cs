using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace quizQaKeTelegramBot.DataBase.Models;

[PrimaryKey(nameof(Id))]
public class User
{
    public Guid Id { get; set; }
    public string? TelegramLogin { get; set; }
    [Required] public long ChatId { get; set; }
}
