using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Contract.FileServices;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;
using WarehouseManagementSystem.Models.Dtos.ProductDtos;
using WarehouseManagementSystem.Models.Responses;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IAsyncRepository<Product> _ProductRepository;
        private readonly IFileService _FileService;
        private readonly IMapper _mapper;

        public ProductController(IMapper mapper, IAsyncRepository<Product> ProductRepository, IFileService FileService)
        {
            _mapper = mapper;
            _ProductRepository = ProductRepository;
            _FileService = FileService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto product)
        {
            var ProductToAdd = _mapper.Map<Product>(product);

            ProductToAdd.Image = product.ProductImage == null
                ? string.Empty
                : await _FileService.SaveFileAsync(product.ProductImage);

            await _ProductRepository.AddAsync(ProductToAdd);

            return Ok(new BaseResponse<object>("تم إضافة المنتج بنجاح", true, 200));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateProductDto product, int ProductId)
        {
            var ProductToUpdate = await _ProductRepository.GetByIdAsync(ProductId);

            if (ProductToUpdate == null)
            {
                return NotFound(new BaseResponse<object>("المنتج غير موجود", true, 404));
            }

            var Image = ProductToUpdate.Image;

            _mapper.Map(product, ProductToUpdate, typeof(UpdateProductDto), typeof(Product));

            ProductToUpdate.Image = product.UpdateOnImage == true
                ? await _FileService.SaveFileAsync(product.ProductImage!)
                : Image;

            await _ProductRepository.UpdateAsync(ProductToUpdate);

            return Ok(new BaseResponse<object>("تم تعديل المنتج بنجاح", true, 200));
        }

        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            var ProductToDelete = await _ProductRepository.GetByIdAsync(UserId);

            if (ProductToDelete == null)
            {
                return NotFound(new BaseResponse<object>("المنتج غير موجود", true, 404));
            }

            await _ProductRepository.DeleteAsync(ProductToDelete);

            return Ok(new BaseResponse<object>("تم حذف المنتج", true, 200));
        }

        [HttpGet("{ProductId}")]
        public async Task<IActionResult> GetProductById(int ProductId)
        {
            var Product = await _ProductRepository.GetByIdAsync(ProductId);

            if (Product == null)
            {
                return NotFound(new BaseResponse<object>("المنتج غير موجود", true, 404));
            }

            var ProductDto = _mapper.Map<ProductDto>(Product);

            return Ok(new BaseResponse<ProductDto>("", true, 200, ProductDto));
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts([FromQuery] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var Products = await _ProductRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            var ProductDtos = _mapper.Map<IEnumerable<ProductDto>>(Products);

            int Count = _ProductRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return Ok(new BaseResponse<IEnumerable<ProductDto>>("", true, 200, ProductDtos, pagination));
        }
    }
}
