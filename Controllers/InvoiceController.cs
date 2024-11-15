using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Constants;
using WarehouseManagementSystem.Models.Dtos.InvoiceDtos;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<Commissary> _commissaryRepository;
        private readonly IAsyncRepository<Customer> _customerRepository;
        private readonly IAsyncRepository<SalesInvoice> _salesInvoiceRepository;
        private readonly IMapper _mapper;

        public InvoiceController(
            IAsyncRepository<Product> productRepository,
            IAsyncRepository<SalesInvoice> salesInvoiceRepository,
            IAsyncRepository<Commissary> commissaryRepository,
            IAsyncRepository<Customer> customerRepository,
            IMapper mapper
        )
        {
            _productRepository = productRepository;
            _salesInvoiceRepository = salesInvoiceRepository;
            _commissaryRepository = commissaryRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesInvoiceById(int id)
        {
            var salesInvoice = await _salesInvoiceRepository.GetByIdAsync(id);

            if (salesInvoice == null)
            {
                return NotFound($"Sales invoice with ID {id} not found.");
            }

            var salesInvoiceDto = _mapper.Map<SalesInvoiceDto>(salesInvoice);

            return Ok(salesInvoiceDto);
        }

        public async Task<IActionResult> CreateSalesInvoice([FromForm] CreateSelesInvoiceDto input)
        {
            var customer = await GetCustomerById(input.CustomerId);
            if (customer == null)
                return NotFound($"Customer with ID {input.CustomerId} not found.");

            var commissary = await GetCommissaryById(input.CommissaryId);
            if (commissary == null)
                return NotFound($"Commissary with ID {input.CommissaryId} not found.");

            var missingProducts = await GetMissingProducts(input.InvoiceItems);
            if (missingProducts.Any())
                return NotFound($"The following products are missing: {string.Join(", ", missingProducts)}");

            var totalProductsPrice = CalculateTotalProductsPrice(input.InvoiceItems);
            var discountAmount = CalculateDiscount(input.DiscountType, input.DiscountValue, totalProductsPrice);
            var invoiceTotal = totalProductsPrice - discountAmount;

            var previousBalance = customer.Balance;
            var currentBalance = CalculateCurrentBalance(previousBalance, invoiceTotal, input.Payment);

            var salesInvoice = _mapper.Map<SalesInvoice>(input);
            salesInvoice.TotalProductsPrice = totalProductsPrice;
            salesInvoice.InvoiceTotal = invoiceTotal;
            salesInvoice.PreviousBalance = previousBalance;
            salesInvoice.CurrentBalance = currentBalance;

            await UpdateCustomerBalance(customer, currentBalance);
            await SaveSalesInvoice(salesInvoice);
            var salesInvoiceDto = _mapper.Map<SalesInvoiceDto>(salesInvoice);
            return Ok(salesInvoiceDto);
        }

        public async Task<IActionResult> UpdateSalesInvoice(int id, [FromForm] CreateSelesInvoiceDto input)
        {
            var salesInvoice = await _salesInvoiceRepository.GetByIdAsync(id);
            if (salesInvoice == null)
                return NotFound($"Sales invoice with ID {id} not found.");

            var customer = await GetCustomerById(input.CustomerId);
            if (customer == null)
                return NotFound($"Customer with ID {input.CustomerId} not found.");

            var commissary = await GetCommissaryById(input.CommissaryId);
            if (commissary == null)
                return NotFound($"Commissary with ID {input.CommissaryId} not found.");

            var missingProducts = await GetMissingProducts(input.InvoiceItems);
            if (missingProducts.Any())
                return NotFound($"The following products are missing: {string.Join(", ", missingProducts)}");

            var totalProductsPrice = CalculateTotalProductsPrice(input.InvoiceItems);
            var discountAmount = CalculateDiscount(input.DiscountType, input.DiscountValue, totalProductsPrice);
            var invoiceTotal = totalProductsPrice - discountAmount;

            var previousBalance = customer.Balance + salesInvoice.InvoiceTotal - salesInvoice.Payment;
            var currentBalance = CalculateCurrentBalance(previousBalance, invoiceTotal, input.Payment);

            _mapper.Map(input, salesInvoice);
            salesInvoice.TotalProductsPrice = totalProductsPrice;
            salesInvoice.InvoiceTotal = invoiceTotal;
            salesInvoice.PreviousBalance = previousBalance;
            salesInvoice.CurrentBalance = currentBalance;

            await UpdateCustomerBalance(customer, currentBalance);
            await _salesInvoiceRepository.UpdateAsync(salesInvoice);

            var salesInvoiceDto = _mapper.Map<SalesInvoiceDto>(salesInvoice);
            return Ok(salesInvoiceDto);
        }

        private async Task<Customer?> GetCustomerById(int customerId)
        {
            return await _customerRepository.GetByIdAsync(customerId);
        }

        private async Task<Commissary?> GetCommissaryById(int commissaryId)
        {
            return await _commissaryRepository.GetByIdAsync(commissaryId);
        }

        private async Task<List<int>> GetMissingProducts(List<InvoiceItemDto> invoiceItems)
        {
            var productChecks = await Task.WhenAll(invoiceItems.Select(async item =>
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                return new { item.ProductId, IsMissing = product == null };
            }));

            return productChecks.Where(result => result.IsMissing).Select(result => result.ProductId).ToList();
        }

        private decimal CalculateTotalProductsPrice(List<InvoiceItemDto> invoiceItems)
        {
            return invoiceItems.Sum(item => item.Price * item.Quantity);
        }
        // TODO
        private decimal CalculateDiscount(DiscountType? discountType, decimal discountValue, decimal totalProductsPrice)
        {
            if (!discountType.HasValue) return 0;
            return discountValue;
            //return discountType == DiscountType.Percentage
            //    ? totalProductsPrice * (discountValue / 100)
            //    : Math.Min(discountValue, totalProductsPrice);
        }

        private decimal CalculateCurrentBalance(decimal previousBalance, decimal invoiceTotal, decimal payment)
        {
            return previousBalance + invoiceTotal - payment;
        }

        private async Task UpdateCustomerBalance(Customer customer, decimal newBalance)
        {
            customer.Balance = newBalance;
            await _customerRepository.UpdateAsync(customer);
        }
        // TODO
        private async Task SaveSalesInvoice(SalesInvoice salesInvoice)
        {
            await _salesInvoiceRepository.AddAsync(salesInvoice);
            salesInvoice.QRCodeContent = $"InvoiceId: {salesInvoice.Id}";
            await _salesInvoiceRepository.UpdateAsync(salesInvoice);
        }
    }
}
