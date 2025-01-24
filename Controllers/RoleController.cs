using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos;
using WarehouseManagementSystem.Models.Dtos.ProductDtos;
using WarehouseManagementSystem.Models.Responses;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IAsyncRepository<Role> _roleRepository;
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IAsyncRepository<Permission> _permissionRepository;
        private readonly IAsyncRepository<RolePermission> _rolePermissionRepository;

        public RoleController(IAsyncRepository<Role> roleRepository, IAsyncRepository<User> userRepository, IAsyncRepository<Permission> permissionRepository, IAsyncRepository<RolePermission> rolePermissionRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string RoleName)
        {
            var Role = new Role()
            {
                RoleName = RoleName
            };

            await _roleRepository.AddAsync(Role);

            return Ok(new BaseResponse<object>("تم إضافة الدور بنجاح", true, 200));
        }

        [HttpDelete("{RoleId}")]
        public async Task<IActionResult> DeleteRole(int RoleId)
        {
            var Role = await _roleRepository.GetByIdAsync(RoleId);

            if(Role == null)
            {
                return Ok(new BaseResponse<object>("الدور غير موجود", false, 404));
            }
            await _roleRepository.DeleteAsync(Role);

            return Ok(new BaseResponse<object>("تم حذف الدور بنجاح", true, 200));
        }
        [HttpPost("AddPermissionToRole")]
        public async Task<IActionResult> AddPermissionToRole([FromBody] RolePermissionDto rolePermission)
        {
            var Role = await _roleRepository.GetByIdAsync(rolePermission.RoleId);

            if (Role == null)
            {
                return Ok(new BaseResponse<object>("الدور غير موجود", false, 404));
            }
            
            foreach(var permission in rolePermission.PermissionIds)
            {
                var RolePermission = new RolePermission()
                {
                    RoleId = rolePermission.RoleId,
                    PermissionId = permission
                };

              await _rolePermissionRepository.AddAsync(RolePermission);
            }

            return Ok(new BaseResponse<object>("تمت العملية بنجاح", true, 200));
        }
    }
}
