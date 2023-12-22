using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthenaResturantWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class prayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Drinks_DrinkID",
                table: "Products");

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

            migrationBuilder.AlterColumn<int>(
                name: "DrinkID",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "AlcoholPercentage",
                table: "Drinks",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Drinks_DrinkID",
                table: "Products",
                column: "DrinkID",
                principalTable: "Drinks",
                principalColumn: "ID");

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
                name: "FK_Products_Drinks_DrinkID",
                table: "Products");

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

            migrationBuilder.AlterColumn<int>(
                name: "DrinkID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AlcoholPercentage",
                table: "Drinks",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Drinks_DrinkID",
                table: "Products",
                column: "DrinkID",
                principalTable: "Drinks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

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
