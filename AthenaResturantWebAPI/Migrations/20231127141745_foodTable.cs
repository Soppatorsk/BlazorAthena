using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthenaResturantWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class foodTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Foods_FoodID",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "FoodID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Foods_FoodID",
                table: "Products",
                column: "FoodID",
                principalTable: "Foods",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Foods_FoodID",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "FoodID",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Foods_FoodID",
                table: "Products",
                column: "FoodID",
                principalTable: "Foods",
                principalColumn: "ID");
        }
    }
}
