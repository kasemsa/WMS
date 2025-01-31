using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.DataBase;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;
using WarehouseManagementSystem.Models.Dtos.CustomerDtos;
using WarehouseManagementSystem.Models.Dtos.UserDtos;
using WarehouseManagementSystem.Models.Responses;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAsyncRepository<User> _UserRepository;
        private readonly IAsyncRepository<UserRole> _userRoleRepository;
        private readonly IAsyncRepository<UserPermission> _userPermissionRepository;
        private readonly IAsyncRepository<RolePermission> _rolePermissionRepository;
        private readonly IAsyncRepository<UserToken> _userTokenRepository;
        private readonly IMapper _mapper;

        public UserController(IAsyncRepository<UserToken> userTokenRepository, IAsyncRepository<RolePermission> rolePermissionRepository, IAsyncRepository<UserPermission> userPermissionRepository, IAsyncRepository<UserRole> userRoleRepository, IMapper mapper, IAsyncRepository<User> UserRepository)
        {
            _mapper = mapper;
            _UserRepository = UserRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _userTokenRepository = userTokenRepository;
        }
        
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
        {
            if(user.Password != user.ConfirmPassword)
            {
                return Ok(new BaseResponse<object>("كلمة السر غير متطابقة", false, 400));
            }

            var UserToAdd = _mapper.Map<User>(user);

            byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

            UserToAdd.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: UserToAdd.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            var User = await _UserRepository.AddAsync(UserToAdd);

            if (user.RoleIds != null)
            {
                foreach (var role in user.RoleIds)
                {
                    await _userRoleRepository.AddAsync(new UserRole()
                    {
                        UserId = User.Id,
                        RoleId = role
                    });
                }
            }
            return Ok(new BaseResponse<object>("تم إنشاء المستخدم بنجاح", true, 200));
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto user, int UserId)
        {
            var UserToUpdate = await _UserRepository.GetByIdAsync(UserId);
            
            if(UserToUpdate == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }
            _mapper.Map(user, UserToUpdate, typeof(UpdateUserDto), typeof(User));

            await _UserRepository.UpdateAsync(UserToUpdate);

            await _userRoleRepository.DeleteRange(r => r.UserId == UserId);

            if(user.RoleIds != null)
            {
                foreach(var role in user.RoleIds)
                {
                    var UserRole = new UserRole()
                    {
                        RoleId = role,
                        UserId = UserId
                    };
                    await _userRoleRepository.AddAsync(UserRole);
                }
            }

            var UserToken = _userTokenRepository.Where(u => u.UserId == UserId).First();

            if (UserToken != null)
            {
                await _userTokenRepository.DeleteAsync(UserToken);
            }
            

            return Ok(new BaseResponse<object>("تم تعديل المستخدم بنجاح", true, 200));
        }

        [HttpDelete("DeleteUser/{UserId}")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            var UserToDelete = await _UserRepository.GetByIdAsync(UserId);

            if (UserToDelete == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }

            await _UserRepository.DeleteAsync(UserToDelete);

            return Ok(new BaseResponse<object>("تم حذف المستخدم", true, 200));
        }

        [HttpGet("GetUserById/{UserId}")]
        public async Task<IActionResult> GetUserById(int UserId)
        {
            var User = await _UserRepository.GetByIdAsync(UserId);
            
            if (User == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }

            var Roles = _userRoleRepository.Where(u => u.UserId == User.Id).Select(u => u.Role).ToList();
            var UserPermissions = _userPermissionRepository.Where(u => u.UserId == User.Id).Select(u => u.Permission).ToList();
            var RolePermissions = _rolePermissionRepository.Where(u=>Roles.Contains(u.Role)).Select(u=>u.Permission).ToList();

            var Permissions = new List<Permission>();

            Permissions.AddRange(RolePermissions);
            Permissions.AddRange(UserPermissions);

            var UserDto = _mapper.Map<UserDto>(User);
            
            UserDto.Roles = Roles;
            UserDto.Permissions = Permissions;

            return Ok(new BaseResponse<UserDto>("", true, 200, UserDto));
        }

        [HttpPost("GetAllUsers")]
        public async Task<BaseResponse<IEnumerable<UserDto>>> GetAllUsers([FromBody] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var Users = await _UserRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            var UsersDtos = _mapper.Map<IEnumerable<UserDto>>(Users);

            foreach(var user in UsersDtos)
            {
                var Roles = _userRoleRepository.Where(u => u.UserId == user.Id).Select(u => u.Role).ToList();
                var UserPermissions = _userPermissionRepository.Where(u => u.UserId == user.Id).Select(u => u.Permission).ToList();
                var RolePermissions = _rolePermissionRepository.Where(u => Roles.Contains(u.Role)).Select(u => u.Permission).ToList();

                var Permissions = new List<Permission>();

                Permissions.AddRange(RolePermissions);
                Permissions.AddRange(UserPermissions);

                user.Roles = Roles;
                user.Permissions = Permissions;
            }

            int Count = _UserRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return new BaseResponse<IEnumerable<UserDto>>("", true, 200, UsersDtos, pagination);
        }
    }
}
