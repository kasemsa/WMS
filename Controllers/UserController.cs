﻿using AutoMapper;
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
                return BadRequest("كلمة السر غير متطابقة");
            }

            var UserToAdd = _mapper.Map<User>(user);

            await _UserRepository.AddAsync(UserToAdd);

            return Ok("تم إنشاء المستخدم بنجاح");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto user, int UserId)
        {
            var UserToUpdate = await _UserRepository.GetByIdAsync(UserId);
            
            if(UserToUpdate == null)
            {
                return NotFound("المستخدم غير موجود");
            }
            _mapper.Map(user, UserToUpdate, typeof(UpdateUserDto), typeof(User));

            await _UserRepository.UpdateAsync(UserToUpdate);

            return Ok("تم إنشاء المستخدم بنجاح");
        }

        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUser(int UserId)
        {
            var UserToDelete = await _UserRepository.GetByIdAsync(UserId);

            if (UserToDelete == null)
            {
                return NotFound("المستخدم غير موجود");
            }

            await _UserRepository.DeleteAsync(UserToDelete);

            return Ok("تم حذف المستخدم");
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUserById(int UserId)
        {
            var User = await _UserRepository.GetByIdAsync(UserId);
            if (User == null)
            {
                return NotFound("المستخدم غير موجود");
            }
            var UserDto = _mapper.Map<UserDto>(User);
            return Ok(UserDto);
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var Users = await _UserRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            var UsersDtos = _mapper.Map<IEnumerable<UserDto>>(Users);

            int Count = _UserRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return Ok(new BaseResponse<IEnumerable<UserDto>>("", true, 200, UsersDtos, pagination));
        }
    }
}
