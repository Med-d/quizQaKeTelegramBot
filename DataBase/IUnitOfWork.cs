using quizQaKeTelegramBot.DataBase.Models;
using quizQaKeTelegramBot.DataBase.Repositories;

namespace quizQaKeTelegramBot.DataBase;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Question> Questions { get; }
    IRepository<Answer> Answers { get; }
    int SaveChanges();
}
