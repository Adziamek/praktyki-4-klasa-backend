using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Warehouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Producent elektroniki", "Samsung" },
                    { 2, "Producent AGD i narzędzi", "Bosch" },
                    { 3, "Producent akcesoriów komputerowych", "Logitech" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Sprzęt elektroniczny", "Elektronika" },
                    { 2, "Sprzęt gospodarstwa domowego", "AGD" },
                    { 3, "Akcesoria komputerowe i inne", "Akcesoria" }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Code", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "WH-01", "Główny magazyn centralny", true, "Magazyn Główny Warszawa" },
                    { 2, "WH-02", "Magazyn regionalny", true, "Magazyn Wrocław" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Code", "IsActive", "Name", "WarehouseId" },
                values: new object[,]
                {
                    { 1, "A-001", true, "Regał A1", 1 },
                    { 2, "A-002", true, "Regał A2", 1 },
                    { 3, "B-001", true, "Regał B1", 2 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "Ean", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), 1, 1, "5901234123457", "Samsung Galaxy Buds" },
                    { new Guid("11111111-1111-1111-1111-111111111102"), 1, 1, "5901234123458", "Samsung Smart TV 50\"" },
                    { new Guid("11111111-1111-1111-1111-111111111103"), 2, 2, "5901234123459", "Bosch Robot kuchenny" },
                    { new Guid("11111111-1111-1111-1111-111111111104"), 2, 2, "5901234123460", "Bosch Ekspres do kawy" },
                    { new Guid("11111111-1111-1111-1111-111111111105"), 3, 3, "5901234123461", "Logitech Mysz MX Master" },
                    { new Guid("11111111-1111-1111-1111-111111111106"), 3, 3, "5901234123462", "Logitech Klawiatura K380" },
                    { new Guid("11111111-1111-1111-1111-111111111107"), 1, 2, "5901234123463", "Samsung Pralka EcoBubble" },
                    { new Guid("11111111-1111-1111-1111-111111111108"), 2, 3, "5901234123464", "Bosch Wiertarka udarowa" },
                    { new Guid("11111111-1111-1111-1111-111111111109"), 3, 1, "5901234123465", "Logitech Kamera internetowa C920" },
                    { new Guid("11111111-1111-1111-1111-111111111110"), 1, 1, "5901234123466", "Samsung Monitor 27\"" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111101"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111102"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111103"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111104"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111105"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111106"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111107"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111108"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111109"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111110"));

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
