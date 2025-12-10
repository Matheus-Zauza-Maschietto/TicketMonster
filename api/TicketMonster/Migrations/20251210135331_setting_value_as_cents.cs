using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketMonster.Migrations
{
    /// <inheritdoc />
    public partial class setting_value_as_cents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "ValueInCents",
                table: "Payments",
                type: "integer",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueInCents",
                table: "Payments");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Payments",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
