using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;
using WarehouseManagementSystem.Models.Dtos.CommissaryDtos;
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

        public CustomerController(IAsyncRepository<Customer> customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        // GET: api/Customer
        //
        //public async Task<IActionResult> GetAllCustomers()
        //{
        //    var customers = await _customerRepository.ListAllAsync();
        //    var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
        //    return Ok(customerDtos);
        //}
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers([FromQuery] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var Customers = await _customerRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            var CustomersDtos = _mapper.Map<IEnumerable<CustomerDto>>(Customers);

            int Count = _customerRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return Ok(new BaseResponse<IEnumerable<CustomerDto>>("", true, 200, CustomersDtos, pagination));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound("الزبون غير موجود");
            }
            var customerDto = _mapper.Map<CustomerDto>(customer);
            return Ok(customerDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = _mapper.Map<Customer>(createCustomerDto);
            await _customerRepository.AddAsync(customer);
            return Ok("تم إضافة الزبون بنجاح");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto updateCustomerDto)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
            {
                return NotFound("الزبون غير موجود");
            }

            _mapper.Map(updateCustomerDto, existingCustomer);
            await _customerRepository.UpdateAsync(existingCustomer);
            return Ok("تم تعديل الزبون بنجاح");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound("الزبون غير موجود");
            }

            await _customerRepository.DeleteAsync(customer);
            return Ok("تم حذف الزبون");
        }
    }
}
