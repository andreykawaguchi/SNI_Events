using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNI_Events.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class fix_dinner_scheduledEvent_relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DinnerScheduledEvent");

            migrationBuilder.AddColumn<long>(
                name: "DinnerId",
                table: "ScheduledEvent",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvent_DinnerId",
                table: "ScheduledEvent",
                column: "DinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledEvent_Dinners_DinnerId",
                table: "ScheduledEvent",
                column: "DinnerId",
                principalTable: "Dinners",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledEvent_Dinners_DinnerId",
                table: "ScheduledEvent");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledEvent_DinnerId",
                table: "ScheduledEvent");

            migrationBuilder.DropColumn(
                name: "DinnerId",
                table: "ScheduledEvent");

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

            migrationBuilder.CreateIndex(
                name: "IX_DinnerScheduledEvent_ScheduledEventsId",
                table: "DinnerScheduledEvent",
                column: "ScheduledEventsId");
        }
    }
}
