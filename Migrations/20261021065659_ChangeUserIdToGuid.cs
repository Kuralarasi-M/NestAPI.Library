using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LibraryApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFcmTokens_Users_UserId1",
                table: "UserFcmTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserFcmTokens_UserId1",
                table: "UserFcmTokens");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserFcmTokens");

            // Drop foreign key constraints before altering columns
            migrationBuilder.DropForeignKey(name: "FK_UserFcmTokens_Users_UserId", table: "UserFcmTokens");
            migrationBuilder.DropForeignKey(name: "FK_Notifications_Users_UserId", table: "Notifications");
            migrationBuilder.DropForeignKey(name: "FK_Favorites_Users_UserId", table: "Favorites");
            migrationBuilder.DropForeignKey(name: "FK_ReadingHistories_Users_UserId", table: "ReadingHistories");
            migrationBuilder.DropForeignKey(name: "FK_SearchHistories_Users_UserId", table: "SearchHistories");
            migrationBuilder.DropForeignKey(name: "FK_DownloadHistories_Users_UserId", table: "DownloadHistories");

            // PostgreSQL requires dropping IDENTITY before changing column type to UUID
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""Id"" TYPE uuid USING (gen_random_uuid());");

            migrationBuilder.Sql(@"ALTER TABLE ""UserFcmTokens"" ALTER COLUMN ""UserId"" TYPE uuid USING (gen_random_uuid());");
            migrationBuilder.Sql(@"ALTER TABLE ""Notifications"" ALTER COLUMN ""UserId"" TYPE uuid USING (gen_random_uuid());");
            migrationBuilder.Sql(@"ALTER TABLE ""Favorites"" ALTER COLUMN ""UserId"" TYPE uuid USING (gen_random_uuid());");
            migrationBuilder.Sql(@"ALTER TABLE ""ReadingHistories"" ALTER COLUMN ""UserId"" TYPE uuid USING (gen_random_uuid());");
            migrationBuilder.Sql(@"ALTER TABLE ""SearchHistories"" ALTER COLUMN ""UserId"" TYPE uuid USING (gen_random_uuid());");
            migrationBuilder.Sql(@"ALTER TABLE ""DownloadHistories"" ALTER COLUMN ""UserId"" TYPE uuid USING (gen_random_uuid());");

            // Re-add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_UserFcmTokens_Users_UserId",
                table: "UserFcmTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Users_UserId",
                table: "Favorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReadingHistories_Users_UserId",
                table: "ReadingHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchHistories_Users_UserId",
                table: "SearchHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DownloadHistories_Users_UserId",
                table: "DownloadHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserFcmTokens",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserFcmTokens",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SearchHistories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ReadingHistories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Notifications",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Favorites",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DownloadHistories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_UserFcmTokens_UserId1",
                table: "UserFcmTokens",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFcmTokens_Users_UserId1",
                table: "UserFcmTokens",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
