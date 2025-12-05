using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixChangeAdminSubscriptionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_AdminId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Admins");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_AdminId",
                table: "Subscriptions",
                column: "AdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_AdminId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionId",
                table: "Admins",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: new Guid("7d555faf-06b9-409f-a3ba-60d2a6bfc228"),
                column: "SubscriptionId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_AdminId",
                table: "Subscriptions",
                column: "AdminId",
                unique: true);
        }
    }
}
