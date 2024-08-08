using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendChallenge.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "bike_id",
                table: "rentals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "total_cost",
                table: "rentals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "fine_cost_percentage_per_day",
                table: "plans",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "account_id",
                table: "delivery_people",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_rentals_bike_id",
                table: "rentals",
                column: "bike_id");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_people_account_id",
                table: "delivery_people",
                column: "account_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_delivery_people_asp_net_users_account_id",
                table: "delivery_people",
                column: "account_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rentals_bikes_bike_id",
                table: "rentals",
                column: "bike_id",
                principalTable: "bikes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_delivery_people_asp_net_users_account_id",
                table: "delivery_people");

            migrationBuilder.DropForeignKey(
                name: "fk_rentals_bikes_bike_id",
                table: "rentals");

            migrationBuilder.DropIndex(
                name: "ix_rentals_bike_id",
                table: "rentals");

            migrationBuilder.DropIndex(
                name: "ix_delivery_people_account_id",
                table: "delivery_people");

            migrationBuilder.DropColumn(
                name: "bike_id",
                table: "rentals");

            migrationBuilder.DropColumn(
                name: "total_cost",
                table: "rentals");

            migrationBuilder.DropColumn(
                name: "fine_cost_percentage_per_day",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "delivery_people");
        }
    }
}
