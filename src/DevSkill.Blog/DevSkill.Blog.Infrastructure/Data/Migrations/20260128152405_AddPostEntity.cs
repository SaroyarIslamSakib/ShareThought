using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DevSkill.Blog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogArea_AspNetUsers_UserId",
                table: "BlogArea");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogArea",
                table: "BlogArea");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e13b3f4a-7c4b-4d2a-9f3b-1c2d3e4f5a6b"));

            migrationBuilder.RenameTable(
                name: "BlogArea",
                newName: "BlogAreas");

            migrationBuilder.RenameIndex(
                name: "IX_BlogArea_UserId",
                table: "BlogAreas",
                newName: "IX_BlogAreas_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "BlogAreas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogAreas",
                table: "BlogAreas",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_BlogAreas_BlogAreaId",
                        column: x => x.BlogAreaId,
                        principalTable: "BlogAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BlogAreaId",
                table: "Posts",
                column: "BlogAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogAreas_AspNetUsers_UserId",
                table: "BlogAreas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogAreas_AspNetUsers_UserId",
                table: "BlogAreas");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogAreas",
                table: "BlogAreas");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "BlogAreas");

            migrationBuilder.RenameTable(
                name: "BlogAreas",
                newName: "BlogArea");

            migrationBuilder.RenameIndex(
                name: "IX_BlogAreas_UserId",
                table: "BlogArea",
                newName: "IX_BlogArea_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogArea",
                table: "BlogArea",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851"), "d290f1ee-6c54-4b01-90e6-d701748f0851", null, "Admin", "ADMIN" },
                    { new Guid("e13b3f4a-7c4b-4d2a-9f3b-1c2d3e4f5a6b"), "e13b3f4a-7c4b-4d2a-9f3b-1c2d3e4f5a6b", null, "Blogger", "BLOGGER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BlogArea_AspNetUsers_UserId",
                table: "BlogArea",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
