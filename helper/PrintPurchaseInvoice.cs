using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Helper
{
    public class PrintPurchaseInvoice(PurchaseInvoice model, IHttpContextAccessor httpContextAccessor) : IDocument
    {
        public PurchaseInvoice Model { get; } = model;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public string BaseUrl
        {
            get
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                return request == null
                    ? throw new InvalidOperationException("HttpContext or Request is not available.")
                    : $"{request.Scheme}://{request.Host}/api/";
            }
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
                    column.Item().Text($"Purchase Invoice #{Model.Id}").FontSize(20).Bold();
                    column.Item().Text($"Commissary: {Model.Commissary.Name}");
                    column.Item().Text($"Date: {Model.CreatedAt:d}");
                });
                row.ConstantItem(100).Height(100).Image(ImageQRCodeHelper.GenerateQRCode($"{BaseUrl}/purchase/{Model.Id}"));
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Items Purchased:").Bold();
                column.Item().Element(ComposeTable);
                column.Item().AlignRight().Text($"Total Items: {Model.InvoiceItems.Count}").Bold();
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
                    header.Cell().Text("Item Name");
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
