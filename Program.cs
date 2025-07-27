using Deployf.Botf;
using quizQaKeTelegramBot.Configure;

BotfProgram.StartBot(args, onConfigure: Startup.ConfigureServices);