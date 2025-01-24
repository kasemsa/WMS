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
        private readonly IMapper _mapper;

        public UserController(IMapper mapper, IAsyncRepository<User> UserRepository)
        {
            _mapper = mapper;
            _UserRepository = UserRepository;
        }
        
        [HttpPost]
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

            await _UserRepository.AddAsync(UserToAdd);

            return Ok(new BaseResponse<object>("تم إنشاء المستخدم بنجاح", true, 200));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto user, int UserId)
        {
            var UserToUpdate = await _UserRepository.GetByIdAsync(UserId);
            
            if(UserToUpdate == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }
            _mapper.Map(user, UserToUpdate, typeof(UpdateUserDto), typeof(User));

            await _UserRepository.UpdateAsync(UserToUpdate);

            return Ok(new BaseResponse<object>("تم تعديل المستخدم بنجاح", true, 200));
        }

        [HttpDelete("{UserId}")]
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

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUserById(int UserId)
        {
            var User = await _UserRepository.GetByIdAsync(UserId);
            if (User == null)
            {
                return Ok(new BaseResponse<object>("المستخدم غير موجود", false, 404));
            }
            var UserDto = _mapper.Map<UserDto>(User);
            return Ok(new BaseResponse<UserDto>("", true, 200, UserDto));
        }

        [HttpGet("GetAllUsers")]
        public async Task<BaseResponse<IEnumerable<UserDto>>> GetAllUsers([FromQuery] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var Users = await _UserRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            var UsersDtos = _mapper.Map<IEnumerable<UserDto>>(Users);

            int Count = _UserRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return new BaseResponse<IEnumerable<UserDto>>("", true, 200, UsersDtos, pagination);
        }
    }
}
