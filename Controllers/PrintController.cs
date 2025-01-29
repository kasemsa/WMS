using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Helper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.InvoiceDtos;
using WarehouseManagementSystem.Models.Responses;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintController : ControllerBase
    {
        private readonly IAsyncRepository<SalesInvoice> _salesInvoiceRepository;
        private readonly IAsyncRepository<PurchaseInvoice> _purchaseInvoiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PrintController(IAsyncRepository<SalesInvoice> salesInvoiceRepository, IAsyncRepository<PurchaseInvoice> purchaseInvoiceRepository, IHttpContextAccessor httpContextAccessor)
        {
            _salesInvoiceRepository = salesInvoiceRepository;
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("PrintSalesInvoiceById/{id}")]
        public async Task<IActionResult> PrintSalesInvoiceById(int id)
        {
            // Fetch the sales invoice by ID
            var salesInvoice = await _salesInvoiceRepository.GetByIdAsync(id);

            // Check if the sales invoice exists
            if (salesInvoice == null)
            {
                return Ok(new BaseResponse<SalesInvoiceDto>(
                    message: $"Sales invoice with ID {id} not found.",
                    success: false,
                    statusCode: 404
                ));
            }

            // Generate the PDF document using the PrintSalesInvoice helper
            var document = new PrintSalesInvoice(salesInvoice, _httpContextAccessor);
            var pdfStream = new MemoryStream();
            document.GeneratePdf(pdfStream);
            pdfStream.Position = 0; // Reset the stream position to the beginning

            // Return the PDF file as a downloadable response
            return File(pdfStream, "application/pdf", $"SalesInvoice_{id}.pdf");
        }

        [HttpGet("PrintPurchaseInvoiceById/{id}")]
        public async Task<IActionResult> PrintPurchaseInvoiceById(int id)
        {
            var purchaseInvoice = await _purchaseInvoiceRepository.GetByIdAsync(id);

            if (purchaseInvoice == null)
            {
                // Return a 404 response with the BaseResponse object
                return Ok(new BaseResponse<PurchaseInvoiceDto>(
                    message: $"Purchase invoice with ID {id} not found.",
                    success: false,
                    statusCode: 404
                ));
            }

            // Generate the PDF document using the PrintPurchaseInvoice helper
            var document = new PrintPurchaseInvoice(purchaseInvoice, _httpContextAccessor);
            var pdfStream = new MemoryStream();
            document.GeneratePdf(pdfStream);
            pdfStream.Position = 0; // Reset the stream position to the beginning

            // Return the PDF file as a downloadable response
            return File(pdfStream, "application/pdf", $"PurchaseInvoice_{id}.pdf");
        }
    }
}