using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;
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
        private readonly IAsyncRepository<UserRole> _userRoleRepository;
        private readonly IAsyncRepository<UserPermission> _userPermissionRepository;
        private readonly IAsyncRepository<Permission> _permissionRepository;
        private readonly IAsyncRepository<RolePermission> _rolePermissionRepository;
        private readonly IAsyncRepository<UserToken> _userTokenRepository;

        public RoleController(IAsyncRepository<UserPermission> userPermissionRepository, IAsyncRepository<UserToken> userTokenRepository, IAsyncRepository<UserRole> userRoleRepository, IAsyncRepository<Role> roleRepository, IAsyncRepository<User> userRepository, IAsyncRepository<Permission> permissionRepository, IAsyncRepository<RolePermission> rolePermissionRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userTokenRepository = userTokenRepository;
            _userPermissionRepository = userPermissionRepository;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string RoleName)
        {
            var Role = new Role()
            {
                RoleName = RoleName
            };

            await _roleRepository.AddAsync(Role);

            return Ok(new BaseResponse<object>("تم إضافة الدور بنجاح", true, 200));
        }

        [HttpPost("AsignRoleToUser")]
        public async Task<IActionResult> AsignRoleToUser([FromBody] AsignRoleToUserRequset requset)
        {
            var Role = await _roleRepository.FindAsync(r=>r.Id == requset.RoleId);
            var User = await _userRepository.FindAsync(r=>r.Id == requset.UserId);

            if(Role == null)
            {
                return Ok(new BaseResponse<object>("الدور غير موجود", false, 404));
            }
            
            if(User == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }

            //var UserRoleCheck = _userRoleRepository.Where(u => u.UserId == User.Id && u.RoleId == Role.Id).First();
            
            //if(UserRoleCheck != null)
            //{
            //    return Ok(new BaseResponse<object>("لا يمكن إسناد دور لهذا المستخدم", false, 400));
            //}
            
            var UserRole = new UserRole()
            {
                RoleId = requset.RoleId,
                UserId = requset.UserId
            };

            await _userRoleRepository.AddAsync(UserRole);

            var UserToken = _userTokenRepository.Where(u => u.UserId == User.Id).FirstOrDefault();
            
            if(UserToken != null)
            {
                await _userTokenRepository.DeleteAsync(UserToken);
            }

            return Ok(new BaseResponse<object>("تمت العملية بنجاح", true, 200));
        }

        [HttpPost("AsignPermissionToUser")]
        public async Task<IActionResult> AsignPermissionToUser([FromBody] AsignPermissionToUserRequset requset)
        {
            var User = await _userRepository.FindAsync(r => r.Id == requset.UserId);

            if (User == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }

            await _userPermissionRepository.DeleteRange(r => r.UserId == User.Id);

            foreach (var permission in requset.PermissionId)
            {
                var UserPermission = new UserPermission()
                {
                    UserId = requset.UserId,
                    PermissionId = permission
                };

                await _userPermissionRepository.AddAsync(UserPermission);
            }

            var UserToken =  _userTokenRepository.Where(u=>u.UserId == User.Id).First();

            await _userTokenRepository.DeleteAsync(UserToken);

            return Ok(new BaseResponse<object>("تمت العملية بنجاح", true, 200));
        }

        [HttpDelete("RemoveRoleFromUser")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] AsignRoleToUserRequset requset)
        {
            var Role = await _roleRepository.FindAsync(r => r.Id == requset.RoleId);
            var User = await _userRepository.FindAsync(r => r.Id == requset.RoleId);

            if (Role == null)
            {
                return Ok(new BaseResponse<object>("الدور غير موجود", false, 404));
            }

            if (User == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }

            var UserRole = _userRoleRepository.Where(r=>r.UserId == requset.UserId && r.RoleId == requset.RoleId).FirstOrDefault();

            if(UserRole == null)
            await _userRoleRepository.DeleteAsync(UserRole!);

            var UserToken = _userTokenRepository.Where(u => u.UserId == User.Id).FirstOrDefault();

            if (UserToken != null)
            {
                await _userTokenRepository.DeleteAsync(UserToken);
            }

            return Ok(new BaseResponse<object>("تمت العملية بنجاح", true, 200));
        }


        [HttpDelete("DeleteRole/{RoleId}")]
        public async Task<IActionResult> DeleteRole(int RoleId)
        {
            var Role = await _roleRepository.GetByIdAsync(RoleId);

            if(Role == null)
            {
                return Ok(new BaseResponse<object>("الدور غير موجود", false, 404));
            }
            await _roleRepository.DeleteAsync(Role);

            var Users = _userRoleRepository.Where(r => r.RoleId == RoleId).Select(r => r.UserId).ToList();

            await _userTokenRepository.DeleteRange(u => Users.Contains(u.UserId));

            return Ok(new BaseResponse<object>("تم حذف الدور بنجاح", true, 200));
        }

        [HttpPost("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles([FromBody] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var Roles = await _roleRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            int Count = _roleRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return Ok(new BaseResponse<object>("", true, 200, Roles, pagination));
        }

        [HttpGet("GetAllPermissions")]
        public async Task<IActionResult> GetAllPermissions([FromQuery] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };
         
            var Permissions = await _permissionRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            int Count = _permissionRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return Ok(new BaseResponse<object>("", true, 200, Permissions, pagination));
        }

        [HttpPut("UpdatePermissionForRole")] 
        public async Task<IActionResult> AddPermissionToRole([FromBody] RolePermissionDto rolePermission)
        {
            var Role = await _roleRepository.GetByIdAsync(rolePermission.RoleId);

            if (Role == null)
            {
                return Ok(new BaseResponse<object>("الدور غير موجود", false, 404));
            }

            await _rolePermissionRepository.DeleteRange(r => r.RoleId == Role.Id);

            foreach(var permission in rolePermission.PermissionIds)
            {
                var RolePermission = new RolePermission()
                {
                    RoleId = rolePermission.RoleId,
                    PermissionId = permission
                };

                await _rolePermissionRepository.AddAsync(RolePermission);
            }

            var Users = _userRoleRepository.Where(r => r.RoleId == rolePermission.RoleId).Select(r => r.UserId).ToList();

            await _userTokenRepository.DeleteRange(u => Users.Contains(u.UserId));
            
            return Ok(new BaseResponse<object>("تمت العملية بنجاح", true, 200));
        }
    }
}
