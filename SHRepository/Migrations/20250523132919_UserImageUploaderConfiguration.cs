using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHServices.Migrations
{
    /// <inheritdoc />
    public partial class UserImageUploaderConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$e4Datz2mMKw0hbMx0Sk0/O26EnebyieLQn9RlK2YYjlEcCktdViSK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$oFAJccxK0G8eK6pNkpDpjuRyT7S5mgrUKUXP788K9fGnj3kii5sz2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$/8LruyiUHNcV0DJ1tCa0HeXx08haulViPpUqeaBITHSpKLMdUKjfq");
        }
    }
}
