using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOU.Migrations
{
    /// <inheritdoc />
    public partial class AddedAvatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSession_Sessions_SessionId",
                table: "NotificationSession");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSession_Users_UserId",
                table: "NotificationSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationSession",
                table: "NotificationSession");

            migrationBuilder.RenameTable(
                name: "NotificationSession",
                newName: "notificationsession");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationSession_UserId",
                table: "notificationsession",
                newName: "IX_notificationsession_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationSession_SessionId",
                table: "notificationsession",
                newName: "IX_notificationsession_SessionId");

            migrationBuilder.AddColumn<int>(
                name: "Avatar",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_notificationsession",
                table: "notificationsession",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_notificationsession_Sessions_SessionId",
                table: "notificationsession",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notificationsession_Users_UserId",
                table: "notificationsession",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notificationsession_Sessions_SessionId",
                table: "notificationsession");

            migrationBuilder.DropForeignKey(
                name: "FK_notificationsession_Users_UserId",
                table: "notificationsession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notificationsession",
                table: "notificationsession");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "notificationsession",
                newName: "NotificationSession");

            migrationBuilder.RenameIndex(
                name: "IX_notificationsession_UserId",
                table: "NotificationSession",
                newName: "IX_NotificationSession_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_notificationsession_SessionId",
                table: "NotificationSession",
                newName: "IX_NotificationSession_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationSession",
                table: "NotificationSession",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSession_Sessions_SessionId",
                table: "NotificationSession",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSession_Users_UserId",
                table: "NotificationSession",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
