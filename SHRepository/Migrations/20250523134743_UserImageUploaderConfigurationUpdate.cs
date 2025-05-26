using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHServices.Migrations
{
    /// <inheritdoc />
    public partial class UserImageUploaderConfigurationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Media",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$ZpPVIH8Ry/b1MOXOWyjab.8luI3OzDB8ZVmZv1Evxq3Izl4xJsJ6G");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$e1K5bnToXz1PM4nuChce4eqhlInSpl4.HOj6YfYH.B4HPAvEwyjAm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$d3jo1LFzr4GUk5F/bBlguOdWz606hG6/UMmuCccy/w4Ob7.sEmWqm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Media");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$E2Mt0eT/MgGKzmxR/yFAXupgkOGIwul7r4TOu/SV/DxVciN9U8Fum");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$yM8iMJFqzFZaw7R820Vi8.F0FkwUrL9.DddNlAQWAgO6CKT/ZzEpe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$BU2bb0M0fR0.1hmO9RKy3uojX6W.sOAlBr2bO3Uq09466b4Ja5IaG");
        }
    }
}
