﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Constants;
using WarehouseManagementSystem.Models.Dtos.InvoiceDtos;
using WarehouseManagementSystem.Models.Responses;

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
        [HttpGet("sales/{id}")]
        public async Task<BaseResponse<SalesInvoiceDto>> GetSalesInvoiceById(int id)
        {
            var salesInvoice = await _salesInvoiceRepository.GetByIdAsync(id);

            if (salesInvoice == null)
            {
                return new BaseResponse<SalesInvoiceDto>(
                    message: $"Sales invoice with ID {id} not found.",
                    success: false,
                    statusCode: 404,
                    data: null
                );
            }

            var salesInvoiceDto = _mapper.Map<SalesInvoiceDto>(salesInvoice);

            return new BaseResponse<SalesInvoiceDto>(
                message: "Sales invoice retrieved successfully.",
                success: true,
                statusCode: 200,
                data: salesInvoiceDto
            );
        }

        [HttpGet("purchase/{id}")]
        public async Task<BaseResponse<PurchaseInvoiceDto>> GetPurchaseInvoiceById(int id)
        {
            var purchaseInvoice = await _purchaseInvoiceRepository.GetByIdAsync(id);

            if (purchaseInvoice == null)
            {
                return new BaseResponse<PurchaseInvoiceDto>(
                    message: $"Purchase invoice with ID {id} not found.",
                    success: false,
                    statusCode: 404,
                    data: null
                );
            }

            var purchaseInvoiceDto = _mapper.Map<PurchaseInvoiceDto>(purchaseInvoice);

            return new BaseResponse<PurchaseInvoiceDto>(
                message: "Purchase invoice retrieved successfully.",
                success: true,
                statusCode: 200,
                data: purchaseInvoiceDto
            );
        }



        #endregion

        #region Refund 
        [HttpPost("refund/sales/{id}")]
        public async Task<BaseResponse<string>> RefundInvoice(int id)
        {
            // Retrieve the invoice
            var invoice = await _salesInvoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                return new BaseResponse<string>(
                    message: $"Invoice with ID {id} not found.",
                    success: false,
                    statusCode: 404
                );
            }

            // Check if already refunded
            if (invoice.Refunded)
            {
                return new BaseResponse<string>(
                    message: "Invoice has already been refunded.",
                    success: false,
                    statusCode: 400
                );
            }

            // Adjust Customer Balance
            var customer = invoice.Customer;
            customer.Balance -= invoice.CurrentBalance; // Reverse the balance impact
            await _customerRepository.UpdateAsync(customer);

            var commissary = invoice.Commissary;

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

            return new BaseResponse<string>(
                message: "Invoice refunded successfully. Products returned to the commissary.",
                success: true,
                statusCode: 200
            );
        }



        [HttpPost("refund/sales")]
        public async Task<BaseResponse<string>> RefundPartialInvoice([FromBody] RefundItemDto input)
        {
            // Retrieve the customer based on CustomerId
            var customer = await _customerRepository.GetByIdAsync(input.CustomerId);
            if (customer == null)
            {
                return new BaseResponse<string>(
                    message: $"Customer with ID {input.CustomerId} not found.",
                    success: false,
                    statusCode: 404
                );
            }

            var commissary = await GetCommissaryById(input.CommissaryId);
            if (commissary == null)
            {
                return new BaseResponse<string>(
                    message: $"Commissary with ID {input.CommissaryId} not found.",
                    success: false,
                    statusCode: 404
                );
            }

            decimal totalRefundAmount = 0;

            // Iterate through each item in the refund request
            foreach (var refundItem in input.InvoiceItems)
            {
                var totalPurchasedQuantity = customer.SalesInvoices
                    .SelectMany(invoice => invoice.InvoiceItems)
                    .Where(item => item.ProductId == refundItem.ProductId)
                    .Sum(item => item.Quantity * (int)item.Unit); // Convert all quantities to base units

                var refundQuantity = refundItem.Quantity * (int)refundItem.Unit;

                if (totalPurchasedQuantity == 0)
                {
                    return new BaseResponse<string>(
                        message: $"Product ID {refundItem.ProductId} was not purchased by the customer.",
                        success: false,
                        statusCode: 400
                    );
                }

                if (refundQuantity > totalPurchasedQuantity)
                {
                    return new BaseResponse<string>(
                        message: $"Refund quantity exceeds the purchased quantity for Product ID {refundItem.ProductId}.",
                        success: false,
                        statusCode: 400
                    );
                }

                var productInvoices = customer.SalesInvoices
                    .SelectMany(invoice => invoice.InvoiceItems)
                    .Where(item => item.ProductId == refundItem.ProductId)
                    .OrderByDescending(item => item.Quantity * (int)item.Unit)
                    .ToList();

                decimal refundAmount = 0;
                foreach (var invoiceItem in productInvoices)
                {
                    if (refundQuantity == 0) break;

                    var invoiceItemQuantity = invoiceItem.Quantity * (int)invoiceItem.Unit;

                    var quantityToRefund = Math.Min(invoiceItemQuantity, refundQuantity);
                    var quantityToRefundInOriginalUnit = quantityToRefund / (int)invoiceItem.Unit;

                    refundAmount += quantityToRefund * (invoiceItem.Price / (int)invoiceItem.Unit);

                    refundQuantity -= quantityToRefund;

                    var commissaryItem = commissary.InvoiceItems
                        .FirstOrDefault(ci => ci.ProductId == refundItem.ProductId);
                    if (commissaryItem != null)
                    {
                        commissaryItem.Quantity += quantityToRefundInOriginalUnit;
                    }
                    else
                    {
                        commissary.InvoiceItems.Add(new InvoiceItem
                        {
                            ProductId = refundItem.ProductId,
                            Quantity = quantityToRefundInOriginalUnit,
                            Unit = invoiceItem.Unit,
                            Price = invoiceItem.Price
                        });
                    }
                }

                totalRefundAmount += refundAmount;
            }

            customer.Balance -= totalRefundAmount;

            await _customerRepository.UpdateAsync(customer);
            await _commissaryRepository.UpdateAsync(commissary);

            return new BaseResponse<string>(
                message: "Partial refund processed successfully.",
                success: true,
                statusCode: 200
            );
        }

        [HttpPost("refund/purchase")]
        public async Task<BaseResponse<string>> RefundPurchase([FromBody] CreatePurchaseInvoiceDto refundRequest)
        {
            var commissary = await _commissaryRepository.GetByIdAsync(refundRequest.CommissaryId);
            if (commissary == null)
            {
                return new BaseResponse<string>(
                    message: $"Commissary with ID {refundRequest.CommissaryId} not found.",
                    success: false,
                    statusCode: 404
                );
            }

            decimal totalRefundAmount = 0;

            foreach (var refundItem in refundRequest.InvoiceItems)
            {
                if (refundItem.Quantity <= 0)
                {
                    return new BaseResponse<string>(
                        message: $"Invalid quantity for Product ID {refundItem.ProductId}. Quantity must be greater than zero.",
                        success: false,
                        statusCode: 400
                    );
                }

                var inventoryItem = commissary.InvoiceItems
                    .FirstOrDefault(item => item.ProductId == refundItem.ProductId);

                if (inventoryItem == null)
                {
                    return new BaseResponse<string>(
                        message: $"Product ID {refundItem.ProductId} not found in Commissary inventory.",
                        success: false,
                        statusCode: 400
                    );
                }

                int refundQuantityInBaseUnit = refundItem.Quantity * (int)refundItem.Unit;

                decimal refundAmount = inventoryItem.Price * refundQuantityInBaseUnit;
                totalRefundAmount += refundAmount;

                inventoryItem.Quantity -= refundQuantityInBaseUnit;

                if (inventoryItem.Quantity == 0)
                {
                    commissary.InvoiceItems.Remove(inventoryItem);
                }
            }

            await _commissaryRepository.UpdateAsync(commissary);

            return new BaseResponse<string>(
                message: "Purchase refund processed successfully.",
                success: true,
                statusCode: 200
            );
        }


        #endregion

        #region Create
        [HttpPost("sales")]
        public async Task<BaseResponse<SalesInvoiceDto>> CreateSalesInvoice([FromForm] CreateSelesInvoiceDto input)
        {
            try
            {
                // Validate customer existence
                var customer = await GetCustomerById(input.CustomerId);
                if (customer == null)
                {
                    return new BaseResponse<SalesInvoiceDto>(
                        message: $"Customer with ID {input.CustomerId} not found.",
                        success: false,
                        statusCode: 404
                    );
                }

                // Validate commissary existence
                var commissary = await GetCommissaryById(input.CommissaryId);
                if (commissary == null)
                {
                    return new BaseResponse<SalesInvoiceDto>(
                        message: $"Commissary with ID {input.CommissaryId} not found.",
                        success: false,
                        statusCode: 404
                    );
                }

                // Validate product existence and commissary stock
                foreach (var item in input.InvoiceItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        return new BaseResponse<SalesInvoiceDto>(
                            message: $"Product with ID {item.ProductId} not found.",
                            success: false,
                            statusCode: 404
                        );
                    }

                    // Check if the commissary has enough stock in terms of the product's unit
                    var commissaryStock = commissary.InvoiceItems
                        .FirstOrDefault(ci => ci.ProductId == item.ProductId);

                    if (commissaryStock == null || commissaryStock.Quantity * (int)commissaryStock.Unit < item.Quantity * (int)item.Unit)
                    {
                        return new BaseResponse<SalesInvoiceDto>(
                            message: $"Not enough stock for Product ID {item.ProductId} in Commissary ID {input.CommissaryId}. Requested quantity exceeds available stock.",
                            success: false,
                            statusCode: 400
                        );
                    }

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

                // Save commissary updates
                await _commissaryRepository.UpdateAsync(commissary);

                // Save the sales invoice
                await _salesInvoiceRepository.AddAsync(salesInvoice);
                await _salesInvoiceRepository.UpdateAsync(salesInvoice);

                // Return the sales invoice DTO
                var salesInvoiceDto = _mapper.Map<SalesInvoiceDto>(salesInvoice);
                return new BaseResponse<SalesInvoiceDto>(
                    message: "Sales invoice created successfully.",
                    success: true,
                    statusCode: 201,
                    data: salesInvoiceDto
                );
            }
            catch (Exception ex)
            {
                // Log exception here if needed
                return new BaseResponse<SalesInvoiceDto>(
                    message: $"An error occurred: {ex.Message}",
                    success: false,
                    statusCode: 500
                );
            }
        }



        [HttpPost("purchase")]
        public async Task<BaseResponse<PurchaseInvoiceDto>> CreatePurchaseInvoice([FromBody] CreatePurchaseInvoiceDto input)
        {
            try
            {
                if (input.InvoiceItems.Count == 0)
                {
                    return new BaseResponse<PurchaseInvoiceDto>(
                        message: "The invoice must include at least one item.",
                        success: false,
                        statusCode: 400
                    );
                }

                // Validate Commissary
                var commissary = await _commissaryRepository.GetByIdAsync(input.CommissaryId);
                if (commissary == null)
                {
                    return new BaseResponse<PurchaseInvoiceDto>(
                        message: $"Commissary with ID {input.CommissaryId} not found.",
                        success: false,
                        statusCode: 404
                    );
                }

                // Initialize variables for processing
                var invoiceItems = new List<InvoiceItem>();

                foreach (var item in input.InvoiceItems)
                {
                    // Validate Product
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        return new BaseResponse<PurchaseInvoiceDto>(
                            message: $"Product with ID {item.ProductId} not found.",
                            success: false,
                            statusCode: 404
                        );
                    }

                    // Prepare the invoice item
                    invoiceItems.Add(new InvoiceItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        PurchaseInvoiceId = null
                    });
                }

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
                    InvoiceItems = invoiceItems
                };

                await _purchaseInvoiceRepository.AddAsync(purchaseInvoice);
                await _commissaryRepository.UpdateAsync(commissary);

                var purchaseInvoiceDto = _mapper.Map<PurchaseInvoiceDto>(purchaseInvoice);

                return new BaseResponse<PurchaseInvoiceDto>(
                    message: "Purchase invoice created successfully.",
                    success: true,
                    statusCode: 201,
                    data: purchaseInvoiceDto
                );
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return new BaseResponse<PurchaseInvoiceDto>(
                    message: $"An error occurred: {ex.Message}",
                    success: false,
                    statusCode: 500
                );
            }
        }

        #endregion

        #region Update
        [HttpPost("sales/{id}")]
        public async Task<BaseResponse<SalesInvoiceDto>> UpdateSalesInvoice(int id, [FromForm] CreateSelesInvoiceDto input)
        {
            try
            {
                var salesInvoice = await _salesInvoiceRepository.GetByIdAsync(id);
                if (salesInvoice == null)
                {
                    return new BaseResponse<SalesInvoiceDto>(
                        message: $"Sales invoice with ID {id} not found.",
                        success: false,
                        statusCode: 404
                    );
                }

                var customer = await GetCustomerById(input.CustomerId);
                if (customer == null)
                {
                    return new BaseResponse<SalesInvoiceDto>(
                        message: $"Customer with ID {input.CustomerId} not found.",
                        success: false,
                        statusCode: 404
                    );
                }

                var commissary = await GetCommissaryById(input.CommissaryId);
                if (commissary == null)
                {
                    return new BaseResponse<SalesInvoiceDto>(
                        message: $"Commissary with ID {input.CommissaryId} not found.",
                        success: false,
                        statusCode: 404
                    );
                }

                var missingProducts = await GetMissingProducts(input.InvoiceItems);
                if (missingProducts.Any())
                {
                    return new BaseResponse<SalesInvoiceDto>(
                        message: $"The following products are missing: {string.Join(", ", missingProducts)}",
                        success: false,
                        statusCode: 404
                    );
                }

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
                return new BaseResponse<SalesInvoiceDto>(
                    message: "Sales invoice updated successfully.",
                    success: true,
                    statusCode: 200,
                    data: salesInvoiceDto
                );
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return new BaseResponse<SalesInvoiceDto>(
                    message: $"An error occurred: {ex.Message}",
                    success: false,
                    statusCode: 500
                );
            }
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
