using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "problems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    company = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    difficulty = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    tags_json = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_problems", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "solutions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    problem_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    language = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    explanation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    is_canonical = table.Column<bool>(type: "bit", nullable: false),
                    quality_score = table.Column<int>(type: "int", nullable: false),
                    source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_solutions", x => x.id);
                    table.ForeignKey(
                        name: "fk_solutions_problems_problem_id",
                        column: x => x.problem_id,
                        principalTable: "problems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_solutions_problem_id",
                table: "solutions",
                column: "problem_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "solutions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "problems");
        }
    }
}
