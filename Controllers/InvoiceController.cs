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
        #region Init
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<Commissary> _commissaryRepository;
        private readonly IAsyncRepository<Customer> _customerRepository;
        private readonly IAsyncRepository<SalesInvoice> _salesInvoiceRepository;
        private readonly IAsyncRepository<PurchaseInvoice> _purchaseInvoiceRepository;
        private readonly IMapper _mapper;

        public InvoiceController(
            IAsyncRepository<Product> productRepository,
            IAsyncRepository<SalesInvoice> salesInvoiceRepository,
            IAsyncRepository<Commissary> commissaryRepository,
            IAsyncRepository<Customer> customerRepository,
            IAsyncRepository<PurchaseInvoice> purchaseInvoiceRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _salesInvoiceRepository = salesInvoiceRepository;
            _commissaryRepository = commissaryRepository;
            _customerRepository = customerRepository;
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _mapper = mapper;
        }
        #endregion

        #region Get
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
        #endregion

        #region Refund 
        [HttpPost("refund/sales/{id}")]
        public async Task<IActionResult> RefundInvoice(int id)
        {
            // Retrieve the invoice
            var invoice = await _salesInvoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                return NotFound($"Invoice with ID {id} not found.");
            }

            // Check if already refunded
            if (invoice.Refunded)
            {
                return BadRequest("Invoice has already been refunded.");
            }

            // Adjust Customer Balance
            var customer = invoice.Customer;
            customer.Balance -= invoice.CurrentBalance; // Reverse the balance impact
            await _customerRepository.UpdateAsync(customer);

            // Adjust Commissary Balance
            var commissary = invoice.Commissary;
            commissary.Balance -= invoice.InvoiceTotal; // Reverse the commissary's earnings

            // Return products to commissary's stock
            foreach (var item in invoice.InvoiceItems)
            {
                var commissaryItem = commissary.InvoiceItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);

                if (commissaryItem != null)
                {
                    // If the product exists in the commissary's stock, increase the quantity
                    commissaryItem.Quantity += item.Quantity;
                }
                else
                {
                    // If the product doesn't exist in the commissary stock, add it
                    commissary.InvoiceItems.Add(new InvoiceItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Price = item.Price
                    });
                }
            }

            // Update the commissary's inventory and balance
            await _commissaryRepository.UpdateAsync(commissary);

            // Mark invoice as refunded
            invoice.Refunded = true;
            await _salesInvoiceRepository.UpdateAsync(invoice);

            return Ok("Invoice refunded successfully. Products returned to the commissary.");
        }

        [HttpPost("refund/sales")]
        public async Task<IActionResult> RefundPartialInvoice([FromBody] RefundItemDto input)
        {
            // Retrieve the customer based on CustomerId
            var customer = await _customerRepository.GetByIdAsync(input.CustomerId)
                ?? throw new ArgumentException($"Customer with ID {input.CustomerId} not found.");

            var commissary = await GetCommissaryById(input.CommissaryId)
                ?? throw new ArgumentException($"Commissary with ID {input.CommissaryId} not found.");

            // List to store the details of the products that will be refunded
            decimal totalRefundAmount = 0;

            // Iterate through each item in the refund request
            foreach (var refundItem in input.InvoiceItems)
            {
                // Calculate total purchased quantity of the product in base units (PIECE)
                var totalPurchasedQuantity = customer.SalesInvoices
                    .SelectMany(invoice => invoice.InvoiceItems)
                    .Where(item => item.ProductId == refundItem.ProductId)
                    .Sum(item => item.Quantity * (int)item.Unit); // Convert all quantities to base units

                // Convert the requested refund quantity to base units
                var refundQuantity = refundItem.Quantity * (int)refundItem.Unit;

                // Check if the customer purchased the product
                if (totalPurchasedQuantity == 0)
                {
                    return BadRequest($"Product ID {refundItem.ProductId} was not purchased by the customer.");
                }

                // Check if the requested refund quantity exceeds the purchased quantity
                if (refundQuantity > totalPurchasedQuantity)
                {
                    return BadRequest($"Refund quantity exceeds the purchased quantity for Product ID {refundItem.ProductId}.");
                }

                // Get all invoices that contain the product (for refund processing)
                var productInvoices = customer.SalesInvoices
                    .SelectMany(invoice => invoice.InvoiceItems)
                    .Where(item => item.ProductId == refundItem.ProductId)
                    .OrderByDescending(item => item.Quantity * (int)item.Unit) // Prioritize larger units
                    .ToList();

                decimal refundAmount = 0;
                foreach (var invoiceItem in productInvoices)
                {
                    if (refundQuantity == 0)
                        break; // Exit once we have refunded the full requested quantity

                    // Convert invoice item's quantity to base units
                    var invoiceItemQuantity = invoiceItem.Quantity * (int)invoiceItem.Unit;

                    // Calculate how much of this product can be refunded from this invoice
                    var quantityToRefund = Math.Min(invoiceItemQuantity, refundQuantity);

                    // Convert refunded quantity back to the original unit for updating inventory
                    var quantityToRefundInOriginalUnit = quantityToRefund / (int)invoiceItem.Unit;

                    refundAmount += quantityToRefund * (invoiceItem.Price / (int)invoiceItem.Unit); // Adjust price based on unit

                    refundQuantity -= quantityToRefund;  // Decrease the remaining quantity to be refunded

                    // Restocking logic: Add refunded quantity back to the commissary inventory
                    var commissaryItem = commissary.InvoiceItems
                        .FirstOrDefault(ci => ci.ProductId == refundItem.ProductId);
                    if (commissaryItem != null)
                    {
                        commissaryItem.Quantity += quantityToRefundInOriginalUnit; // Increase commissary stock
                    }
                    else
                    {
                        commissary.InvoiceItems.Add(new InvoiceItem
                        {
                            ProductId = refundItem.ProductId,
                            Quantity = quantityToRefundInOriginalUnit,
                            Unit = invoiceItem.Unit,  // Maintain the unit from the original sale
                            Price = invoiceItem.Price
                        });
                    }
                }

                totalRefundAmount += refundAmount;
            }

            // Adjust balances for the customer and commissary
            customer.Balance -= totalRefundAmount;
            commissary.Balance -= totalRefundAmount;

            // Save the updated records
            await _customerRepository.UpdateAsync(customer);
            await _commissaryRepository.UpdateAsync(commissary);

            return Ok(new { message = "Partial refund processed successfully." });
        }

        [HttpPost("refund/purchase")]
        public async Task<IActionResult> RefundPurchase([FromBody] CreatePurchaseInvoiceDto refundRequest)
        {
            // Retrieve commissary
            var commissary = await _commissaryRepository.GetByIdAsync(refundRequest.CommissaryId);
            if (commissary == null)
                return NotFound($"Commissary with ID {refundRequest.CommissaryId} not found.");

            decimal totalRefundAmount = 0;

            foreach (var refundItem in refundRequest.InvoiceItems)
            {
                if (refundItem.Quantity <= 0)
                    return BadRequest($"Invalid quantity for Product ID {refundItem.ProductId}. Quantity must be greater than zero.");

                // Find the product in the commissary's inventory
                var inventoryItem = commissary.InvoiceItems
                    .FirstOrDefault(item => item.ProductId == refundItem.ProductId);

                if (inventoryItem == null)
                    return BadRequest($"Product ID {refundItem.ProductId} not found in Commissary inventory.");

                // Convert refund quantity to base unit (PIECE)
                int refundQuantityInBaseUnit = refundItem.Quantity * (int)refundItem.Unit;


                // Calculate refund amount
                decimal refundAmount = inventoryItem.Price * refundQuantityInBaseUnit;
                totalRefundAmount += refundAmount;

                // Deduct the quantity from commissary's inventory
                inventoryItem.Quantity -= refundQuantityInBaseUnit;

                // If the quantity becomes zero, optionally remove the item
                if (inventoryItem.Quantity == 0)
                {
                    commissary.InvoiceItems.Remove(inventoryItem);
                }
            }

            // Update commissary balance
            commissary.Balance += totalRefundAmount;

            // Save the changes
            await _commissaryRepository.UpdateAsync(commissary);

            return Ok(new
            {
                Message = "Purchase refund processed successfully.",
                TotalRefundAmount = totalRefundAmount
            });
        }

        #endregion

        #region Create

        public async Task<SalesInvoiceDto> CreateSalesInvoice([FromForm] CreateSelesInvoiceDto input)
        {
            // Validate customer existence
            var customer = await GetCustomerById(input.CustomerId)
                ?? throw new ArgumentException($"Customer with ID {input.CustomerId} not found.");

            // Validate commissary existence
            var commissary = await GetCommissaryById(input.CommissaryId)
                ?? throw new ArgumentException($"Commissary with ID {input.CommissaryId} not found.");

            // Validate product existence and commissary stock
            foreach (var item in input.InvoiceItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new ArgumentException($"Product with ID {item.ProductId} not found.");

                // Check if the commissary has enough stock in terms of the product's unit
                var commissaryStock = commissary.InvoiceItems
                    .FirstOrDefault(ci => ci.ProductId == item.ProductId);

                if (commissaryStock == null || commissaryStock.Quantity * (int)commissaryStock.Unit < item.Quantity * (int)item.Unit)
                    throw new ArgumentException($"Not enough stock for Product ID {item.ProductId} in Commissary ID {input.CommissaryId}. Requested quantity exceeds available stock.");

                // Deduct the quantity from the commissary stock
                commissaryStock.Quantity -= item.Quantity * (int)item.Unit; // Adjust for unit multiplier
            }

            // Calculate total price and discount
            var totalProductsPrice = CalculateTotalProductsPrice(input.InvoiceItems);
            var discountAmount = CalculateDiscount(input.DiscountType, input.DiscountValue, totalProductsPrice);
            var invoiceTotal = totalProductsPrice - discountAmount;

            // Calculate balances
            var previousBalance = customer.Balance;
            var currentBalance = CalculateCurrentBalance(previousBalance, invoiceTotal, input.Payment);

            // Create sales invoice entity
            var salesInvoice = CreateSalesInvoiceEntity(input, totalProductsPrice, discountAmount, invoiceTotal, previousBalance, currentBalance);

            // Update customer balance
            await UpdateCustomerBalance(customer, currentBalance);

            // Update commissary balance
            commissary.Balance += invoiceTotal - input.Payment;

            // Save commissary updates
            await _commissaryRepository.UpdateAsync(commissary);

            // Save the sales invoice
            await _salesInvoiceRepository.AddAsync(salesInvoice);
            await _salesInvoiceRepository.UpdateAsync(salesInvoice);

            // Return the sales invoice DTO
            return _mapper.Map<SalesInvoiceDto>(salesInvoice);
        }


        [HttpPost("purchase")]
        public async Task<PurchaseInvoiceDto> CreatePurchaseInvoice([FromBody] CreatePurchaseInvoiceDto input)
        {
            if (input.InvoiceItems.Count == 0)
                throw new ArgumentException("The invoice must include at least one item.");

            // Validate Commissary
            var commissary = await _commissaryRepository.GetByIdAsync(input.CommissaryId)
             ?? throw new ArgumentException($"Commissary with ID {input.CommissaryId} not found.");


            // Initialize variables for processing
            var invoiceItems = new List<InvoiceItem>();
            decimal invoiceTotal = 0;

            foreach (var item in input.InvoiceItems)
            {
                // Validate Product
                var product = await _productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new ArgumentException($"Product with ID {item.ProductId} not found.");


                // Calculate unit price and invoice total
                decimal unitPrice = product.Price;
                decimal totalItemPrice = unitPrice * item.Quantity * (int)item.Unit;
                invoiceTotal += totalItemPrice;

                // Prepare the invoice item
                invoiceItems.Add(new InvoiceItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Price = unitPrice,
                    PurchaseInvoiceId = null
                });
            }

            // Update Commissary Balance
            commissary.Balance -= invoiceTotal;

            // Update Commissary Inventory
            foreach (var invoiceItem in invoiceItems)
            {
                var existingItem = commissary.InvoiceItems
                    .FirstOrDefault(ci => ci.ProductId == invoiceItem.ProductId);

                if (existingItem != null)
                {
                    int existingQuantityInBaseUnit = existingItem.Quantity * (int)existingItem.Unit;

                    // Convert new quantity to base unit (PIECE)
                    int newQuantityInBaseUnit = invoiceItem.Quantity * (int)invoiceItem.Unit;

                    // Add quantities in base unit
                    int totalQuantityInBaseUnit = existingQuantityInBaseUnit + newQuantityInBaseUnit;

                    // Convert back to the original unit of the existing item
                    existingItem.Quantity = totalQuantityInBaseUnit / (int)existingItem.Unit;
                    existingItem.Quantity += invoiceItem.Quantity;
                }
                else
                {
                    commissary.InvoiceItems.Add(new InvoiceItem
                    {
                        ProductId = invoiceItem.ProductId,
                        Quantity = invoiceItem.Quantity,
                        Unit = invoiceItem.Unit,
                        Price = invoiceItem.Price
                    });
                }
            }

            // Save the Purchase Invoice
            var purchaseInvoice = new PurchaseInvoice
            {
                CommissaryId = commissary.Id,
                InvoiceTotal = invoiceTotal,
                InvoiceItems = invoiceItems
            };

            await _purchaseInvoiceRepository.AddAsync(purchaseInvoice);
            await _commissaryRepository.UpdateAsync(commissary);

            return _mapper.Map<PurchaseInvoiceDto>(purchaseInvoice);
        }

        #endregion

        #region Update
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
        #endregion

        #region Private Methods
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

        private SalesInvoice CreateSalesInvoiceEntity(
            CreateSelesInvoiceDto input,
            decimal totalProductsPrice,
            decimal discountAmount,
            decimal invoiceTotal,
            decimal previousBalance,
            decimal currentBalance)
        {
            return new SalesInvoice
            {
                Payment = input.Payment,
                DiscountValue = discountAmount,
                DiscountType = input.DiscountType,
                CustomerId = input.CustomerId,
                CommissaryId = input.CommissaryId,
                TotalProductsPrice = totalProductsPrice,
                InvoiceTotal = invoiceTotal,
                PreviousBalance = previousBalance,
                CurrentBalance = currentBalance,
                InvoiceItems = input.InvoiceItems.Select(item => new InvoiceItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Price = item.Price
                }).ToList()
            };
        }

        #endregion
    }
}
