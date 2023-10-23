using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class fix_chat_relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_receiver",
                table: "chats");

            migrationBuilder.DropForeignKey(
                name: "fk_user_sender",
                table: "chats");

            migrationBuilder.AddForeignKey(
                name: "fk_user_receiver",
                table: "chats",
                column: "receiver",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_sender",
                table: "chats",
                column: "sender",
                principalTable: "users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_receiver",
                table: "chats");

            migrationBuilder.DropForeignKey(
                name: "fk_user_sender",
                table: "chats");

            migrationBuilder.AddForeignKey(
                name: "fk_user_receiver",
                table: "chats",
                column: "receiver",
                principalTable: "fees",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_sender",
                table: "chats",
                column: "sender",
                principalTable: "fees",
                principalColumn: "id");
        }
    }
}
