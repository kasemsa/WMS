using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Helper
{
    public class InvoicePrint : IDocument
    {
        public SalesInvoice Model { get; }

        public InvoicePrint(SalesInvoice model)
        {
            Model = model;
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Bill #{Model.Id}").FontSize(20).Bold();
                    column.Item().Text($"Date: {Model.CreatedBy:d}");
                    //column.Item().Text($"Due Date: {Model.DueDate:d}");
                });

                //row.ConstantItem(100).Height(100).Image(ImageQRCodeHelper.GenerateQRCode(Model.QRCodeContent));
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Text("Bill To:").Bold();
                column.Item().Text(Model.Customer.Name);

                column.Item().Element(ComposeTable);

                column.Item().AlignRight().Text($"Total Amount: ${Model.TotalProductsPrice}").Bold();
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(200);
                    columns.RelativeColumn();
                    columns.ConstantColumn(100);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Name");
                    header.Cell().AlignRight().Text("Quantity");
                    header.Cell().AlignRight().Text("Price");
                });

                foreach (var item in Model.InvoiceItems)
                {
                    table.Cell().Text(item.Product.Name);
                    table.Cell().Text(item.Quantity.ToString());
                    table.Cell().AlignRight().Text($"${item.Price}");
                }
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
                x.Span(" of ");
                x.TotalPages();
            });
        }
    }
}
