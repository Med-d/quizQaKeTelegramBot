using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace quizQaKeTelegramBot.DataBase.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext Context;
    public Repository(DbContext context)
    {
        Context = context;
    }
    public IEnumerable<T> GetAll() => Context.Set<T>().ToList();
    public T? GetById(Guid id) => Context.Set<T>().Find(id);
    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => Context.Set<T>().Where(predicate);
    public void Add(T entity) => Context.Set<T>().Add(entity);
    public void Update(T entity) => Context.Set<T>().Update(entity);
    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}
