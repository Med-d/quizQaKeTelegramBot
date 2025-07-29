using System;
using quizQaKeTelegramBot.DataBase.Models;
using quizQaKeTelegramBot.Services.Models;

namespace quizQaKeTelegramBot.Services;

public class QuestionService
{
    private readonly DataBase.IUnitOfWork unitOfWork;
    private readonly FakeAnswerGenerator fakeAnswerGenerator;

    public QuestionService(DataBase.IUnitOfWork unitOfWork, FakeAnswerGenerator fakeAnswerGenerator)
    {
        this.unitOfWork = unitOfWork;
        this.fakeAnswerGenerator = fakeAnswerGenerator;
    }

    public (Question, Answer)? GetQuestionAndAnswerForUser(User user)
    {
        var pendingAnswer = unitOfWork.Answers.Find(a => a.UserId == user.Id && a.State == AnswerState.Pending).FirstOrDefault();
        if (pendingAnswer != null)
        {
            var foundQuestion = unitOfWork.Questions.GetById(pendingAnswer.QuestionId);
            if (foundQuestion != null)
                return (foundQuestion, pendingAnswer);
        }

        // Получаем список вопросов, на которые пользователь ещё не отвечал
        var answeredIds = unitOfWork.Answers.Find(a => a.UserId == user.Id && (a.State == AnswerState.Answered || a.State == AnswerState.NotAnswered)).Select(a => a.QuestionId).ToHashSet();
        var availableQuestion = unitOfWork.Questions.GetAll().FirstOrDefault(q => !answeredIds.Contains(q.Id));
        if (availableQuestion == null)
            return null;

        var fakeAnswers = fakeAnswerGenerator.GenerateFakeAnswers(3, availableQuestion).ToArray();
        var newAnswer = new Answer
        {
            UserId = user.Id,
            QuestionId = availableQuestion.Id,
            State = AnswerState.Pending,
            AlternativeAnswers = fakeAnswers
        };
        unitOfWork.Answers.Add(newAnswer);
        unitOfWork.SaveChanges();
        return (availableQuestion, newAnswer);
    }

    public CheckAnswerResult? CheckAnswer(User user, string answer)
    {
        var pendingAnswer = unitOfWork.Answers.Find(a => a.UserId == user.Id && a.State == AnswerState.Pending).FirstOrDefault();
        if (pendingAnswer == null)
        {
            return null;
        }
        var question = unitOfWork.Questions.GetById(pendingAnswer.QuestionId);
        if (question == null)
        {
            return null;
        }

        if (answer == question.Answer)
        {
            pendingAnswer.State = AnswerState.Answered;
            pendingAnswer.AnsweredDateTime = DateTime.UtcNow;
            unitOfWork.SaveChanges();
            return new CheckAnswerResult { IsAnswerCorrect = true, RemainingAttempts = 2 - pendingAnswer.Attempt };
        }
        else
        {
            pendingAnswer.Attempt++;
            if (pendingAnswer.Attempt >= 2)
            {
                pendingAnswer.State = AnswerState.NotAnswered;
            }
            unitOfWork.SaveChanges();
            return new CheckAnswerResult { IsAnswerCorrect = false, RemainingAttempts = Math.Max(0, 2 - pendingAnswer.Attempt) };
        }
    }

    public UserScore[] GetUserScores()
    {
        var users = unitOfWork.Users.GetAll().ToList();
        var answers = unitOfWork.Answers.GetAll().ToList();
        var scores = new List<UserScore>();

        foreach (var user in users)
        {
            int totalScore = 0;
            DateTime lastAnswerDateTime = DateTime.MaxValue;
            var userAnswers = answers.Where(a => a.UserId == user.Id && (a.State == AnswerState.Answered || a.State == AnswerState.NotAnswered)).ToList();
            foreach (var ans in userAnswers)
            {
                if (ans.State == AnswerState.Answered)
                {
                    int score = 4 - ans.Attempt;
                    if (ans.UsedFirstClue) score--;
                    if (ans.UsedSecondClue) score--;
                    if (score < 0) score = 0;
                    totalScore += score;
                    if (ans.AnsweredDateTime != null && ans.AnsweredDateTime < lastAnswerDateTime)
                        lastAnswerDateTime = ans.AnsweredDateTime.Value;
                }
            }
            scores.Add(new UserScore
            {
                TelegramLogin = user.TelegramLogin,
                Score = totalScore,
                LastAnswerDateTime = lastAnswerDateTime
            });
        }
        return scores
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.LastAnswerDateTime)
            .ToArray();
    }
}
