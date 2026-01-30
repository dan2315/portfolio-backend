using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class daily_activity_date_as_pk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_daily_activity",
                table: "daily_activity");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "daily_activity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_daily_activity",
                table: "daily_activity",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_daily_activity",
                table: "daily_activity");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "daily_activity",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_daily_activity",
                table: "daily_activity",
                column: "Id");
        }
    }
}
