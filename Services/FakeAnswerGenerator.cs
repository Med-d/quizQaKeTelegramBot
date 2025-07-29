using System;
using System.Collections.Generic;
using quizQaKeTelegramBot.DataBase.Models;

namespace quizQaKeTelegramBot.Services;

public class FakeAnswerGenerator
{
    private readonly Question[] AdditionalNames = [
        new(){
            Fact = "",
            Hobby = "",
            Answer = "Логвинов Максим Олегович",
            FirstClue = "Мужской",
            SecondClue = ""
        },
        new(){
            Fact = "",
            Hobby = "",
            Answer = "Раткин Кирилл Павлович",
            FirstClue = "Мужской",
            SecondClue = ""
        },
        new(){
            Fact = "",
            Hobby = "",
            Answer = "Алиферова Злата Евгеньевна",
            FirstClue = "Женский",
            SecondClue = ""
        }];

    private readonly DataBase.IUnitOfWork unitOfWork;

    public FakeAnswerGenerator(DataBase.IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public List<string> GenerateFakeAnswers(int count, Question forQuestion)
    {
        var available = new List<Question>(AdditionalNames);
        available.AddRange(unitOfWork.Questions.GetAll().Where(q => !q.Answer.Equals(forQuestion.Answer) && !string.IsNullOrWhiteSpace(q.Answer)));

        // Группируем по FirstClue
        var male = available.Where(q => q.FirstClue == "Мужской").ToList();
        var female = available.Where(q => q.FirstClue == "Женский").ToList();

        // Определяем, какой пол у forQuestion
        var preferOpposite = forQuestion.FirstClue == "Мужской" ? female : male;
        var preferSame = forQuestion.FirstClue == "Мужской" ? male : female;

        var rnd = new Random();
        var result = new List<string>();

        var countOpposite = (int)Math.Ceiling(count / 2d);

        // Сначала добавляем ответы с противоположным полом
        foreach (var q in preferOpposite.OrderBy(_ => rnd.Next()).Take(Math.Min(countOpposite, preferOpposite.Count)))
            result.Add(q.Answer);

        // Если не хватает, добавляем с тем же полом
        if (result.Count < count)
        {
            foreach (var q in preferSame.OrderBy(_ => rnd.Next()).Take(count - result.Count))
                result.Add(q.Answer);
        }

        return result;
    }
}
