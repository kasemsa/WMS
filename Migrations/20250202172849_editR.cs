using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class editR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseInvoices_Customers_CommissaryId",
                table: "PurchaseInvoices");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseInvoices_Commissaries_CommissaryId",
                table: "PurchaseInvoices",
                column: "CommissaryId",
                principalTable: "Commissaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseInvoices_Commissaries_CommissaryId",
                table: "PurchaseInvoices");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseInvoices_Customers_CommissaryId",
                table: "PurchaseInvoices",
                column: "CommissaryId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
