using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MehranBot.Migrations
{
    public partial class m1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDateMl = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateSh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WelcomeMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartBotText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RulesMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvertiseFee = table.Column<long>(type: "bigint", nullable: false),
                    BotToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetChannelUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetChannelId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminChatId = table.Column<long>(type: "bigint", nullable: false),
                    AdminUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateMl = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateSh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsConfirmedRules = table.Column<bool>(type: "bit", nullable: false),
                    IsMembership = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateMl = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateSh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FkCategoryId = table.Column<int>(type: "int", nullable: false),
                    FkUserId = table.Column<long>(type: "bigint", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    IsPayed = table.Column<bool>(type: "bit", nullable: false),
                    IsAssigned = table.Column<bool>(type: "bit", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateMl = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateSh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ads_Category_FkCategoryId",
                        column: x => x.FkCategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ads_Users_FkUserId",
                        column: x => x.FkUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FkUserId = table.Column<long>(type: "bigint", nullable: false),
                    ActivityType = table.Column<int>(type: "int", nullable: false),
                    CreateDateMl = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateSh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_Users_FkUserId",
                        column: x => x.FkUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_FkCategoryId",
                table: "Ads",
                column: "FkCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ads_FkUserId",
                table: "Ads",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_FkUserId",
                table: "UserActivities",
                column: "FkUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ads");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
