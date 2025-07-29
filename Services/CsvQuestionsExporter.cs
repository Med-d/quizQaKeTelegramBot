using System;
using quizQaKeTelegramBot.DataBase.Models;

namespace quizQaKeTelegramBot.Services;

public class CsvQuestionsExporter
{
    public Question[] ImportQuestionsFromCsv(string csvPath, char separator = ';')
    {
        var questions = new List<Question>();
        using var reader = new StreamReader(csvPath);
        reader.ReadLine(); // skip header
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var columns = line.Split(separator);
            if (columns.Length < 6) continue;
            var question = new Question
            {
                Fact = columns[3],
                Hobby = columns[4],
                Answer = columns[0],
                FirstClue = columns[1],
                SecondClue = columns[2],
            };
            questions.Add(question);
        }
        return questions.ToArray();
    }
}
