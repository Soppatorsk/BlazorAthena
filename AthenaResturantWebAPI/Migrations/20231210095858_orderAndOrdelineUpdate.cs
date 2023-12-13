using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthenaResturantWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class orderAndOrdelineUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderLines_OrderLineID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderLineID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderLineID",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "OrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderID",
                table: "OrderLines",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Orders_OrderID",
                table: "OrderLines",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Orders_OrderID",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_OrderID",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "OrderLines");

            migrationBuilder.AddColumn<int>(
                name: "OrderLineID",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderLineID",
                table: "Orders",
                column: "OrderLineID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderLines_OrderLineID",
                table: "Orders",
                column: "OrderLineID",
                principalTable: "OrderLines",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
