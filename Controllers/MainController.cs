using System;
using Deployf.Botf;
using quizQaKeTelegramBot.DataBase;
using quizQaKeTelegramBot.DataBase.Models;
using quizQaKeTelegramBot.Services;

namespace quizQaKeTelegramBot.Controllers;

public class MainController(IUnitOfWork unitOfWork, CsvQuestionsExporter csvExporter, QuestionService questionService) : BotController
{
    private record Answering
    {
        public bool IsAnswering { get; set; } = false;
        public required Question Question { get; set; }
        public required Answer Answer { get; set; }
    }

    private record GagUnknown
    {
        public int CountOfUnknowns { get; set; } = 0;
    }

    [State]
    private Answering? answeringState;
    [State]
    private GagUnknown? gagUnknown;
    
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    private readonly CsvQuestionsExporter csvExporter = csvExporter;
    private readonly QuestionService questionService = questionService;

    [Action("/start")]
    public async Task Start()
    {
        var telegramLogin = Context.Update.Message?.Chat.Username;
        var chatId = Context.Update.Message?.Chat.Id;

        // Проверяем, есть ли пользователь в базе
        var existingUser = unitOfWork.Users.Find(u => u.TelegramLogin == telegramLogin).FirstOrDefault();
        if (existingUser != null)
        {
            // Если пользователь уже есть, ничего не делаем
            await Send("Вы уже зарегистрированы. Приступайте к выполнению задания!");
            return;
        }

        // Добавляем нового пользователя
        if (chatId == null)
        {
            // Не удалось получить chatId, не добавляем пользователя
            await Send("Не удалось получить ChatId. Обратись к @mmeddl или организатору");
            return;
        }
        var newUser = new User
        {
            TelegramLogin = telegramLogin,
            ChatId = (long)chatId
        };
        unitOfWork.Users.Add(newUser);
        unitOfWork.SaveChanges();

        // Приветственное сообщение
        await Send(
            "<b>Добро пожаловать на квиз!</b>\n\n" +
            "Вам будут задаваться вопросы о ваших коллегах. На каждый вопрос у вас есть 2 попытки и возможность получить 2 подсказки.\n" +
            "Максимальный балл за вопрос — 4:\n" +
            "• За новую попытку — минус 1 балл\n" +
            "• За каждую подсказку — минус 1 балл\n\n" +
            "Удачи!"
        );
    }

    [Action("/question", "Получить задание")]
    public async Task GetQuestion()
    {
        // Получаем текущего пользователя
        var telegramLogin = Context.Update.Message?.Chat.Username;
        var user = unitOfWork.Users.Find(u => u.TelegramLogin == telegramLogin).FirstOrDefault();
        if (user == null)
        {
            await Send("Пользователь не найден. Зарегистрируйтесь через /start.");
            return;
        }

        // Получаем вопрос и ответ через QuestionService
        var result = questionService.GetQuestionAndAnswerForUser(user);

        if (result is null)
        {
            await Send("Вопросы закончились! Осталось дождаться результатов");
            return;
        }

        var (question, answer) = result.Value;

        // Формируем варианты ответов
        var allAnswers = new List<string> { question.Answer };
        if (answer.AlternativeAnswers != null)
            allAnswers.AddRange(answer.AlternativeAnswers);
        allAnswers = allAnswers.OrderBy(_ => Guid.NewGuid()).ToList();

        // Формируем сообщение с информацией о попытках и подсказках
        var attemptsLeft = 2 - answer.Attempt;
        var cluesUsed = (answer.UsedFirstClue ? 1 : 0) + (answer.UsedSecondClue ? 1 : 0);
        var cluesInfo = $"Использовано подсказок: {cluesUsed}/2";
        var attemptsInfo = $"Осталось попыток: {attemptsLeft}";

        var cluesText = "";
        if (answer.UsedFirstClue)
        {
            cluesText += $"\nПервая подсказка: пол {question.FirstClue}";
        }
        if (answer.UsedSecondClue)
        {
            cluesText += $"\nВторая подсказка: {question.SecondClue}";
        }

        PushLL("<b>Внимание, вопрос! Кто это?</b>");
        PushLL($"{attemptsInfo}");
        PushL($"Факт: {question.Fact}\nХобби: {question.Hobby}\n\n{cluesInfo}{cluesText}");
        Button("Подсказка (-1 балл)", Q(UseClue, question, answer));

        await Send();
        foreach (var ans in allAnswers)
        {
            RowKButton(ans);
        }

        answeringState = new Answering
        {
            IsAnswering = true,
            Question = question,
            Answer = answer
        };
        await Send("Напиши свой ответ! Используй опции вместо клавиатуры");
    }

    [On(Handle.Unknown), Filter(And: Filters.Text)]
    public async Task UnknowOrAnswer()
    {
        if (!answeringState?.IsAnswering ?? true)
        {
            await Unknow();
            return;
        }

        answeringState!.IsAnswering = false;
        var userAnswer = Context.Update.Message?.Text;

        if (userAnswer is null)
        {
            await Send("Не получилось получить ответ");
            return;
        }

        var telegramLogin = Context.Update.Message?.Chat.Username;
        var user = unitOfWork.Users.Find(u => u.TelegramLogin == telegramLogin).FirstOrDefault();
        if (user == null)
        {
            await Send("Пользователь не найден. Зарегистрируйтесь через /start.");
            return;
        }

        var checkResult = questionService.CheckAnswer(user, userAnswer);
        if (checkResult is null)
        {
            await Send("Не получилось проверить ответ. Обратись к @mmeddl или организатору");
            return;
        }
        
        if (checkResult?.IsAnswerCorrect == true)
        {
            await Send("Верно! Вы получаете баллы.");
            await GetQuestion();
        }
        else
        {
            PushL("<b>Неправильно!</b>");
            if (checkResult?.RemainingAttempts > 0)
                PushL("Попробуйте ещё раз введя /question или воспользуйтесь подсказкой.");
            else
                PushL("Попытки закончились. Преходи к следующему вопросу используя /question");
            await Send();
        }
    }

    [On(Handle.Exception)]
    public async Task ExceptionHandler()
    {
        await Send("Произошла какая-то ошибка. Обратись к @mmeddl или организатору");
    }

    [Action]
    public async Task Unknow()
    {
        var tip = "Если пытаешься ответить на вопрос - начни с /question";

        if (gagUnknown is null) gagUnknown = new GagUnknown();

        switch (gagUnknown.CountOfUnknowns)
        {
            case 0:
                await Send($"Ничего не понял. {tip}");
                break;
            case 1:
                await Send($"Кажется ты пытаешься меня протестировать? Не стоит, оставь силы для работы. {tip}");
                break;
            case 2:
                await Send($"Хватит меня тестировать, это плохо закончится! {tip}");
                break;
            case 3:
                await Send($"Я упал. Дамы и господа, бот больше не работает");
                break;
            case 4:
                await Send($"Ты сейчас бан получишь");
                break;
            default:
                await Send($"<b>BAN</b>\n\nВы получили бан за плохое поведение и попытку протестировать бота для квиза\n\n<span class=\"tg-spoiler\">Шутка. Продолжай отвечать введя /question</span>");
                break;
        }

        gagUnknown.CountOfUnknowns++;
    }

    [Action]
    public async Task UseClue(Question question, Answer answer)
    {
        string clueMsg;
        if (!answer.UsedFirstClue)
        {
            answer.UsedFirstClue = true;
            clueMsg = $"Первая подсказка: пол {question.FirstClue}";
        }
        else if (!answer.UsedSecondClue)
        {
            answer.UsedSecondClue = true;
            clueMsg = $"Вторая подсказка: {question.SecondClue}";
        }
        else
        {
            clueMsg = "Вы уже использовали обе подсказки для этого вопроса.";
        }
        unitOfWork.SaveChanges();
        await Send(clueMsg);
    }

    [Action("/leaderboard", "Таблица лидеров")]
    public async Task GetLeaderboard()
    {
        var scores = questionService.GetUserScores();
        if (scores.Length == 0)
        {
            await Send("Нет данных для отображения лидерборда.");
            return;
        }
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<b>Таблица лидеров:</b>");
        for (int i = 0; i < scores.Length; i++)
        {
            var s = scores[i];
            sb.AppendLine($"{i + 1}. @{s.TelegramLogin} - {s.Score}");
        }
        await Send(sb.ToString());
    }

    [Action("/migrate_questions")]
    public async Task MigrateQuestions()
    {
        // Путь к CSV-файлу можно заменить на актуальный
        string csvPath = "./questions.csv";
        var questions = csvExporter.ImportQuestionsFromCsv(csvPath);
        // Очищаем старые вопросы
        foreach (var q in unitOfWork.Questions.GetAll())
        {
            unitOfWork.Questions.Remove(q);
        }
        foreach (var q in questions)
        {
            unitOfWork.Questions.Add(q);
        }
        unitOfWork.SaveChanges();
        await Send($"Questions migrated: {questions.Length}");
    }
}
