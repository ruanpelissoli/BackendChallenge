using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendChallenge.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliverymanEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "delivery_people",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cnh_number = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    cnh_type = table.Column<string>(type: "text", nullable: false),
                    cnh_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_people", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_delivery_people_cnh_number",
                table: "delivery_people",
                column: "cnh_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_delivery_people_cnpj",
                table: "delivery_people",
                column: "cnpj",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_people");
        }
    }
}
