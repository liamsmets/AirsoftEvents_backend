using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirsoftEvents.Persistance.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MolliePaymentId",
                table: "Reservations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MolliePaymentId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Reservations");
        }
    }
}
