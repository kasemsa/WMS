using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceItemsAndBalanceToCommissary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeContent",
                table: "SalesInvoices");

            migrationBuilder.AddColumn<bool>(
                name: "Refunded",
                table: "SalesInvoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CommissaryId",
                table: "InvoiceItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_CommissaryId",
                table: "InvoiceItems",
                column: "CommissaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Commissaries_CommissaryId",
                table: "InvoiceItems",
                column: "CommissaryId",
                principalTable: "Commissaries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Commissaries_CommissaryId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_CommissaryId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "Refunded",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "CommissaryId",
                table: "InvoiceItems");

            migrationBuilder.AddColumn<string>(
                name: "QRCodeContent",
                table: "SalesInvoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
