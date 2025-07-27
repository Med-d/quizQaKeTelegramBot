using Microsoft.EntityFrameworkCore;
using quizQaKeTelegramBot.DataBase.Models;

namespace quizQaKeTelegramBot.DataBase;

public sealed class QuizDbContext(DbContextOptions<QuizDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();
}