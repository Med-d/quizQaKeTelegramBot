using quizQaKeTelegramBot.DataBase.Models;
using quizQaKeTelegramBot.DataBase.Repositories;

namespace quizQaKeTelegramBot.DataBase;

public class UnitOfWork : IUnitOfWork
{
    private readonly QuizDbContext context;
    public IRepository<User> Users { get; }
    public IRepository<Question> Questions { get; }
    public IRepository<Answer> Answers { get; }

    public UnitOfWork(QuizDbContext context)
    {
        this.context = context;
        Users = new Repository<User>(this.context);
        Questions = new Repository<Question>(this.context);
        Answers = new Repository<Answer>(this.context);
    }

    public int SaveChanges() => context.SaveChanges();

    private bool disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
