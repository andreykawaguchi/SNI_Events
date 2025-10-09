using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNI_Events.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDinner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dinners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIdCreated = table.Column<long>(type: "bigint", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserIdModified = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserIdDeleted = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dinners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DinnerScheduledEvent",
                columns: table => new
                {
                    DinnersId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledEventsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinnerScheduledEvent", x => new { x.DinnersId, x.ScheduledEventsId });
                    table.ForeignKey(
                        name: "FK_DinnerScheduledEvent_Dinners_DinnersId",
                        column: x => x.DinnersId,
                        principalTable: "Dinners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DinnerScheduledEvent_ScheduledEvent_ScheduledEventsId",
                        column: x => x.ScheduledEventsId,
                        principalTable: "ScheduledEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDinners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DinnerId = table.Column<long>(type: "bigint", nullable: false),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIdCreated = table.Column<long>(type: "bigint", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserIdModified = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserIdDeleted = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDinners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDinners_Dinners_DinnerId",
                        column: x => x.DinnerId,
                        principalTable: "Dinners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDinners_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DinnerScheduledEvent_ScheduledEventsId",
                table: "DinnerScheduledEvent",
                column: "ScheduledEventsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDinners_DinnerId",
                table: "UserDinners",
                column: "DinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDinners_UserId",
                table: "UserDinners",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DinnerScheduledEvent");

            migrationBuilder.DropTable(
                name: "UserDinners");

            migrationBuilder.DropTable(
                name: "Dinners");
        }
    }
}
