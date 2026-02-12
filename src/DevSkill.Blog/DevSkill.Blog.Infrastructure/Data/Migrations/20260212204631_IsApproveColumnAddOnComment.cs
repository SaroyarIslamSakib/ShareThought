using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevSkill.Blog.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsApproveColumnAddOnComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Comment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Comment");
        }
    }
}
