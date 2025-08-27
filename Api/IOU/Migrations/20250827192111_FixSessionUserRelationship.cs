using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOU.Migrations
{
    /// <inheritdoc />
    public partial class FixSessionUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessionusers_Users_UserId1",
                table: "Sessionusers");

            migrationBuilder.DropIndex(
                name: "IX_Sessionusers_UserId1",
                table: "Sessionusers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Sessionusers");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ExpenseSplits",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExpenseSplits");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Sessionusers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessionusers_UserId1",
                table: "Sessionusers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessionusers_Users_UserId1",
                table: "Sessionusers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
