using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaktlyC.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorTapRound",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ColorTapGameId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorTapRound", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColorTapWordPairDisplay",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Word = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ColorTapRoundId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorTapWordPairDisplay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorTapWordPairDisplay_ColorTapRound_ColorTapRoundId",
                        column: x => x.ColorTapRoundId,
                        principalTable: "ColorTapRound",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrentMiniGameId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreparationDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    RoomId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MiniGames",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoundCount = table.Column<int>(type: "int", nullable: false),
                    CurrentRound = table.Column<int>(type: "int", nullable: false),
                    RoundDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    InstructionsStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstructionsDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGames_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    HostId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentGameId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Games_CurrentGameId",
                        column: x => x.CurrentGameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlayerScores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlayerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerScores_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerScores_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorTapRound_ColorTapGameId",
                table: "ColorTapRound",
                column: "ColorTapGameId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorTapWordPairDisplay_ColorTapRoundId",
                table: "ColorTapWordPairDisplay",
                column: "ColorTapRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_CurrentMiniGameId",
                table: "Games",
                column: "CurrentMiniGameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_RoomId",
                table: "Games",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGames_GameId",
                table: "MiniGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_RoomId",
                table: "Players",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerScores_GameId",
                table: "PlayerScores",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerScores_PlayerId",
                table: "PlayerScores",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CurrentGameId",
                table: "Rooms",
                column: "CurrentGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColorTapRound_MiniGames_ColorTapGameId",
                table: "ColorTapRound",
                column: "ColorTapGameId",
                principalTable: "MiniGames",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_MiniGames_CurrentMiniGameId",
                table: "Games",
                column: "CurrentMiniGameId",
                principalTable: "MiniGames",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Rooms_RoomId",
                table: "Games",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_MiniGames_CurrentMiniGameId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Rooms_RoomId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "ColorTapWordPairDisplay");

            migrationBuilder.DropTable(
                name: "PlayerScores");

            migrationBuilder.DropTable(
                name: "ColorTapRound");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "MiniGames");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
