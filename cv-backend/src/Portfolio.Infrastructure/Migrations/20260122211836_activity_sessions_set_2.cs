using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class activity_sessions_set_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivitySessions",
                table: "ActivitySessions");

            migrationBuilder.RenameTable(
                name: "ActivitySessions",
                newName: "activity_sessions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_activity_sessions",
                table: "activity_sessions",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_activity_sessions",
                table: "activity_sessions");

            migrationBuilder.RenameTable(
                name: "activity_sessions",
                newName: "ActivitySessions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivitySessions",
                table: "ActivitySessions",
                column: "SessionId");
        }
    }
}
