using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetDataProvider.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    XmlId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsLive = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    XmlId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatchType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    XmlId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsLive = table.Column<bool>(type: "bit", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    XmlId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bets_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchChangeMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchChangeMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchChangeMessages_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BetChangeMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BetId = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetChangeMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BetChangeMessages_Bets_BetId",
                        column: x => x.BetId,
                        principalTable: "Bets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Odds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialBetValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BetId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    XmlId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Odds_Bets_BetId",
                        column: x => x.BetId,
                        principalTable: "Bets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OddChangeMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OddId = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OddChangeMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OddChangeMessages_Odds_OddId",
                        column: x => x.OddId,
                        principalTable: "Odds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BetChangeMessages_BetId",
                table: "BetChangeMessages",
                column: "BetId");

            migrationBuilder.CreateIndex(
                name: "IX_Bets_MatchId",
                table: "Bets",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Bets_XmlId",
                table: "Bets",
                column: "XmlId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_SportId",
                table: "Events",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_XmlId",
                table: "Events",
                column: "XmlId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchChangeMessages_MatchId",
                table: "MatchChangeMessages",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_EventId",
                table: "Matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_XmlId",
                table: "Matches",
                column: "XmlId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OddChangeMessages_OddId",
                table: "OddChangeMessages",
                column: "OddId");

            migrationBuilder.CreateIndex(
                name: "IX_Odds_BetId",
                table: "Odds",
                column: "BetId");

            migrationBuilder.CreateIndex(
                name: "IX_Odds_XmlId",
                table: "Odds",
                column: "XmlId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sports_XmlId",
                table: "Sports",
                column: "XmlId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BetChangeMessages");

            migrationBuilder.DropTable(
                name: "MatchChangeMessages");

            migrationBuilder.DropTable(
                name: "OddChangeMessages");

            migrationBuilder.DropTable(
                name: "Odds");

            migrationBuilder.DropTable(
                name: "Bets");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Sports");
        }
    }
}
