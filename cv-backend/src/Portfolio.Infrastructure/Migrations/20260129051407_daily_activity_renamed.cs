using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class daily_activity_renamed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_daily_activities",
                table: "daily_activities");

            migrationBuilder.RenameTable(
                name: "daily_activities",
                newName: "daily_activity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_daily_activity",
                table: "daily_activity",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_daily_activity",
                table: "daily_activity");

            migrationBuilder.RenameTable(
                name: "daily_activity",
                newName: "daily_activities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_daily_activities",
                table: "daily_activities",
                column: "Id");
        }
    }
}
