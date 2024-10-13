using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaktlyC.Migrations
{
    /// <inheritdoc />
    public partial class Round_Scoreboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScoreboardLines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    MiniGameRoundId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreboardLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreboardLines_MiniGameRounds_MiniGameRoundId",
                        column: x => x.MiniGameRoundId,
                        principalTable: "MiniGameRounds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScoreboardLines_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreboardLines_MiniGameRoundId",
                table: "ScoreboardLines",
                column: "MiniGameRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreboardLines_PlayerId",
                table: "ScoreboardLines",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreboardLines");
        }
    }
}
