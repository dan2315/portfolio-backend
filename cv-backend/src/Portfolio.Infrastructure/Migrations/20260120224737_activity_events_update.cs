using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class activity_events_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "activity_events",
                newName: "Referer");

            migrationBuilder.RenameColumn(
                name: "AnonymousSessionId",
                table: "activity_events",
                newName: "SessionId");

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "activity_events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalData",
                table: "activity_events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AnonymousId",
                table: "activity_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TimeOnPageMs",
                table: "activity_events",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalData",
                table: "activity_events");

            migrationBuilder.DropColumn(
                name: "AnonymousId",
                table: "activity_events");

            migrationBuilder.DropColumn(
                name: "TimeOnPageMs",
                table: "activity_events");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "activity_events",
                newName: "AnonymousSessionId");

            migrationBuilder.RenameColumn(
                name: "Referer",
                table: "activity_events",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "activity_events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
