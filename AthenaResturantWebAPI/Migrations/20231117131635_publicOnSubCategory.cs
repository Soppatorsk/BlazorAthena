﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthenaResturantWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class publicOnSubCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SubCategories");
        }
    }
}
