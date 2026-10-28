using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeTrainerEmailPhoneUniqueInGym : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainers_Email",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_GymId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_Phone",
                table: "Trainers");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_GymId_Email",
                table: "Trainers",
                columns: new[] { "GymId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_GymId_Phone",
                table: "Trainers",
                columns: new[] { "GymId", "Phone" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trainers_GymId_Email",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_GymId_Phone",
                table: "Trainers");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Email",
                table: "Trainers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_GymId",
                table: "Trainers",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Phone",
                table: "Trainers",
                column: "Phone",
                unique: true);
        }
    }
}
