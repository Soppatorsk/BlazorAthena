using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthenaResturantWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedMerch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MerchID",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Merch",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merch", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_MerchID",
                table: "Products",
                column: "MerchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Merch_MerchID",
                table: "Products",
                column: "MerchID",
                principalTable: "Merch",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Merch_MerchID",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Merch");

            migrationBuilder.DropIndex(
                name: "IX_Products_MerchID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MerchID",
                table: "Products");
        }
    }
}
