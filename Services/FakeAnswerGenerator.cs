using System;
using System.Collections.Generic;

namespace quizQaKeTelegramBot.Services;

public class FakeAnswerGenerator
{
    private readonly string[] AdditionalNames = ["Логвинов Максим Олегович", "Раткин Кирилл Павлович", "Осипов Алексей Сергеевич"];

    private readonly DataBase.IUnitOfWork unitOfWork;

    public FakeAnswerGenerator(DataBase.IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public List<string> GenerateFakeAnswers(int count)
    {
        var available = new HashSet<string>(AdditionalNames);
        var realAnswers = unitOfWork.Questions.GetAll().Select(q => q.Answer).Where(a => !string.IsNullOrWhiteSpace(a));
        foreach (var ans in realAnswers)
            available.Add(ans);

        var rnd = new Random();
        var result = available.OrderBy(_ => rnd.Next()).Take(count).ToList();
        return result;
    }
}
