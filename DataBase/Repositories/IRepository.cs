using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace quizQaKeTelegramBot.DataBase.Repositories;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(Guid id);
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
