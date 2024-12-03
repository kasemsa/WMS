using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceTotal",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Commissaries");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "RoleName" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 12, 3, 17, 40, 18, 703, DateTimeKind.Utc).AddTicks(154), "System", null, false, null, null, "Admin" },
                    { 2, new DateTime(2024, 12, 3, 17, 40, 18, 703, DateTimeKind.Utc).AddTicks(160), "System", null, false, null, null, "Commissary" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "InvoiceTotal",
                table: "PurchaseInvoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Commissaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
