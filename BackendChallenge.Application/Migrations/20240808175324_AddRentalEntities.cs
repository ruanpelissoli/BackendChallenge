using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendChallenge.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_in_days = table.Column<int>(type: "integer", nullable: false),
                    cost_per_day = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rentals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    deliveryman_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rentals", x => x.id);
                    table.ForeignKey(
                        name: "fk_rentals_deliveryman_deliveryman_id",
                        column: x => x.deliveryman_id,
                        principalTable: "delivery_people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rentals_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_rentals_deliveryman_id",
                table: "rentals",
                column: "deliveryman_id");

            migrationBuilder.CreateIndex(
                name: "ix_rentals_plan_id",
                table: "rentals",
                column: "plan_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rentals");

            migrationBuilder.DropTable(
                name: "plans");
        }
    }
}
