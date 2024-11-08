using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Contract.FileService;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.ProductDtos;
using WarehouseManagementSystem.Models.Dtos.UserDtos;

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

            return Ok("تم إضافة المنتج بنجاح");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateProductDto product, int ProductId)
        {
            var ProductToUpdate = await _ProductRepository.GetByIdAsync(ProductId);

            if (ProductToUpdate == null)
            {
                return NotFound("المنتج غير موجود");
            }

            var Image = ProductToUpdate.Image;

            _mapper.Map(product, ProductToUpdate, typeof(UpdateProductDto), typeof(Product));

            ProductToUpdate.Image = product.UpdateOnImage == true
                ? await _FileService.SaveFileAsync(product.ProductImage!)
                : Image;

            await _ProductRepository.UpdateAsync(ProductToUpdate);

            return Ok("تم تعديل المنتج بنجاح");
        }

        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            var ProductToDelete = await _ProductRepository.GetByIdAsync(UserId);

            if (ProductToDelete == null)
            {
                return NotFound("المنتج غير موجود");
            }

            await _ProductRepository.DeleteAsync(ProductToDelete);

            return Ok("تم حذف المنتج");
        }
    }
}
