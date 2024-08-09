using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendChallenge.Application.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bikes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    year = table.Column<int>(type: "integer", maxLength: 4, nullable: false),
                    model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    license_plate = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bikes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_in_days = table.Column<int>(type: "integer", nullable: false),
                    cost_per_day = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    fine_cost_percentage_per_day = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "deliverymen",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cnh_number = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    cnh_type = table.Column<string>(type: "text", nullable: false),
                    cnh_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deliverymen", x => x.id);
                    table.ForeignKey(
                        name: "fk_deliverymen_asp_net_users_account_id",
                        column: x => x.account_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rentals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bike_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deliveryman_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rentals", x => x.id);
                    table.ForeignKey(
                        name: "fk_rentals_bikes_bike_id",
                        column: x => x.bike_id,
                        principalTable: "bikes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rentals_deliveryman_deliveryman_id",
                        column: x => x.deliveryman_id,
                        principalTable: "deliverymen",
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
                name: "ix_bikes_license_plate",
                table: "bikes",
                column: "license_plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deliverymen_account_id",
                table: "deliverymen",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deliverymen_cnh_number",
                table: "deliverymen",
                column: "cnh_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deliverymen_cnpj",
                table: "deliverymen",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rentals_bike_id",
                table: "rentals",
                column: "bike_id");

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
                name: "bikes");

            migrationBuilder.DropTable(
                name: "deliverymen");

            migrationBuilder.DropTable(
                name: "plans");
        }
    }
}
