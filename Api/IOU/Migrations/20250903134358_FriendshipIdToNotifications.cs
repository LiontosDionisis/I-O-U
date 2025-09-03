using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOU.Migrations
{
    /// <inheritdoc />
    public partial class FriendshipIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FriendshipId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FriendshipId",
                table: "Notifications",
                column: "FriendshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Friendships_FriendshipId",
                table: "Notifications",
                column: "FriendshipId",
                principalTable: "Friendships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Friendships_FriendshipId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_FriendshipId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "FriendshipId",
                table: "Notifications");
        }
    }
}
