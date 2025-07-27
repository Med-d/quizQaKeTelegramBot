using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using quizQaKeTelegramBot.DataBase;
using quizQaKeTelegramBot.Services;

namespace quizQaKeTelegramBot.Configure
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection collection, IConfiguration configuration)
        {
            collection.AddDbContext<QuizDbContext>((provider, options) =>
            {
                options.UseSqlite("Data Source=./photo_quest.db");
            });
            collection.AddScoped<IUnitOfWork, UnitOfWork>();
            collection.AddSingleton<CsvQuestionsExporter>();
            collection.AddSingleton<FakeAnswerGenerator>();
            collection.AddSingleton<QuestionService>();
        }
    }
}