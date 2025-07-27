using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quizQaKeTelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuestionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsedFirstClue = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsedSecondClue = table.Column<bool>(type: "INTEGER", nullable: false),
                    Attempt = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    AnsweredDateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => new { x.UserId, x.QuestionId });
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Fact = table.Column<string>(type: "TEXT", nullable: false),
                    Hobby = table.Column<string>(type: "TEXT", nullable: false),
                    Answer = table.Column<string>(type: "TEXT", nullable: false),
                    FirstClue = table.Column<string>(type: "TEXT", nullable: false),
                    SecondClue = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TelegramLogin = table.Column<string>(type: "TEXT", nullable: true),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
