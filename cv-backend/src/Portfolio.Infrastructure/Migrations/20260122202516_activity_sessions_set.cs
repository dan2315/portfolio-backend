using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class activity_sessions_set : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "activity_events",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "ActivitySessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PagesViewed = table.Column<int>(type: "integer", nullable: false),
                    CartridgesInserted = table.Column<int>(type: "integer", nullable: false),
                    ContactAttempted = table.Column<int>(type: "integer", nullable: false),
                    TotalTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    AdditionalData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySessions", x => x.SessionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivitySessions");

            migrationBuilder.DropColumn(
                name: "Processed",
                table: "activity_events");
        }
    }
}
