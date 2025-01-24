using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;
using WarehouseManagementSystem.Models.Dtos.CustomerDtos;
using WarehouseManagementSystem.Models.Responses;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IAsyncRepository<Customer> _customerRepository;
        private readonly IMapper _mapper;

        public CustomerController(IMapper mapper, IAsyncRepository<Customer> customerRepository)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customerDto)
        {
            // Check if the customer name already exists
            bool nameExists = _customerRepository.Where(c => c.Name == customerDto.Name).Any();
            if (nameExists)
                return Ok(new BaseResponse<CustomerDto>("اسم العميل موجود بالفعل", success: false, statusCode: 400));

            var customer = _mapper.Map<Customer>(customerDto);

            await _customerRepository.AddAsync(customer);

            return Ok(new BaseResponse<CustomerDto>("تم إضافة العميل بنجاح"));
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerDto customerDto, int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                return Ok(new BaseResponse<CustomerDto>("العميل غير موجود", success: false, statusCode: 404));

            // Check if the customer name already exists
            bool nameExists = _customerRepository.Where(c => c.Name == customerDto.Name).Any();
            if (nameExists)
                return Ok(new BaseResponse<CustomerDto>("اسم العميل موجود بالفعل", success: false, statusCode: 400));

            _mapper.Map(customerDto, customer);

            await _customerRepository.UpdateAsync(customer);

            return Ok(new BaseResponse<CustomerDto>("تم تعديل بيانات العميل بنجاح"));
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                return Ok(new BaseResponse<CustomerDto>("العميل غير موجود", success: false, statusCode: 404));

            await _customerRepository.DeleteAsync(customer);

            return Ok(new BaseResponse<CustomerDto>("تم حذف العميل بنجاح"));
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerById(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                return Ok(new BaseResponse<CustomerDto>("العميل غير موجود", success: false, statusCode: 404));

            var customerDto = _mapper.Map<CustomerDto>(customer);

            return Ok(new BaseResponse<CustomerDto>("", success: true, statusCode: 200, data: customerDto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] IndexQuery query)
        {
            var filterObject = new FilterObject { Filters = query.filters };

            var customers = await _customerRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            int totalCount = _customerRepository.WhereThenFilter(_ => true, filterObject).Count();

            if (totalCount == 0)
                return Ok(new BaseResponse<IEnumerable<CustomerDto>>(
                message: "لا توجد بيانات متاحة",
                success: true,
                statusCode: 204
            ));

            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);

            var pagination = new Pagination(query.page, query.perPage, totalCount);

            return Ok(new BaseResponse<IEnumerable<CustomerDto>>(
                message: "",
                success: true,
                statusCode: 200,
                data: customerDtos,
                pagination: pagination
            ));
        }
    }
}
