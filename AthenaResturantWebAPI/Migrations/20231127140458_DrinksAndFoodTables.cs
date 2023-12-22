using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthenaResturantWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class DrinksAndFoodTables : Migration
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
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Lactose",
                table: "Foods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Nuts",
                table: "Foods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AlcoholPercentage",
                table: "Drinks",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Foods_FoodID",
                table: "Products",
                column: "FoodID",
                principalTable: "Foods",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Foods_FoodID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Lactose",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Nuts",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "AlcoholPercentage",
                table: "Drinks");

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
    }
}
