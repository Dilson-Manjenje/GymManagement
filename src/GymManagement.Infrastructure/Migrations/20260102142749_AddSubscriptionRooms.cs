using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionRooms_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionRooms_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionRooms_RoomId",
                table: "SubscriptionRooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionRooms_SubscriptionId_RoomId",
                table: "SubscriptionRooms",
                columns: new[] { "SubscriptionId", "RoomId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionRooms");
        }
    }
}
