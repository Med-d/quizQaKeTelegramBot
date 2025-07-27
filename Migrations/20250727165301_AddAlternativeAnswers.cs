using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quizQaKeTelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class AddAlternativeAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlternativeAnswers",
                table: "Answers",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternativeAnswers",
                table: "Answers");
        }
    }
}
