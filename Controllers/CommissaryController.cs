﻿using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;
using WarehouseManagementSystem.Models.Constants;
using WarehouseManagementSystem.Models.Dtos.CommissaryDtos;
using WarehouseManagementSystem.Models.Responses;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissaryController : ControllerBase
    {
        private readonly IAsyncRepository<Commissary> _commissaryRepository;
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public CommissaryController(
            IMapper mapper,
            IAsyncRepository<Commissary> commissaryRepository,
            IAsyncRepository<User> userRepository)
        {
            _mapper = mapper;
            _commissaryRepository = commissaryRepository;
            _userRepository = userRepository;
        }
        [HttpPost("GetAllCommissaries")]
        public async Task<IActionResult> GetAllCommissaries([FromBody] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var commissaries = await _commissaryRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            var commissaryDtos = _mapper.Map<List<CommissaryDto>>(commissaries);
            
            for(int i = 0; i < commissaries.Count; i++)
            {
                commissaryDtos[i].Email = await _userRepository.Where(u => u.Id == commissaries[i].UserId).Select(u => u.Email).FirstOrDefaultAsync();
            }
            
            int Count = _commissaryRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return Ok(new BaseResponse<IEnumerable<CommissaryDto>>("", true, 200, commissaryDtos, pagination));
        }
        
        [HttpGet("GetCommissaryById/{commissaryId}")]
        public async Task<IActionResult> GetCommissaryById(int commissaryId)
        {
            var commissary = await _commissaryRepository
                .Where(c => c.Id == commissaryId)
                .Include(c => c.User)
                .FirstOrDefaultAsync();

            if (commissary == null)
                return Ok(new BaseResponse<object>("المندوب غير موجود", false, 404));

            var commissaryDto = _mapper.Map<CommissaryDto>(commissary);

            commissaryDto.Email = await _userRepository.Where(u=>u.Id == commissary.UserId).Select(u => u.Email).FirstOrDefaultAsync();
            
            return Ok(new BaseResponse<CommissaryDto>("", true, 200, commissaryDto));
        }

        [HttpPost("CreateCommissary")]
        public async Task<IActionResult> CreateCommissary([FromBody] CreateCommissaryDto commissaryDto)
        {
            if (commissaryDto.Password != commissaryDto.ConfirmPassword)
                return Ok(new BaseResponse<object>("كلمة السر غير متطابقة", false, 400));

            byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

            commissaryDto.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: commissaryDto.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));


            var user = await _userRepository.AddAsync(_mapper.Map<User>(commissaryDto));
            var commissary = _mapper.Map<Commissary>(commissaryDto);
            commissary.UserId = user.Id;

            await _commissaryRepository.AddAsync(commissary);

            return Ok(new BaseResponse<object>("تم إضافة المندوب بنجاح", true, 200));
        }

        [HttpPut("UpdateCommissary/{commissaryId}")]
        public async Task<IActionResult> UpdateCommissary([FromBody] UpdateCommissaryDto commissaryDto, int commissaryId)
        {
            var commissary = await _commissaryRepository.GetByIdAsync(commissaryId);
            if (commissary == null)
                return Ok(new BaseResponse<object>("المندوب غير موجود", false, 404));

            _mapper.Map(commissaryDto, commissary);
            await _commissaryRepository.UpdateAsync(commissary);

            return Ok(new BaseResponse<object>("تم تعديل المندوب بنجاح", true, 200));
        }

        [HttpDelete("DeleteCommissary/{commissaryId}")]
        public async Task<IActionResult> DeleteCommissary(int commissaryId)
        {
            var commissary = await _commissaryRepository.GetByIdAsync(commissaryId);
            if (commissary == null)
                return Ok(new BaseResponse<object>("المندوب غير موجود", false, 404));

            await _commissaryRepository.DeleteAsync(commissary);

            return Ok(new BaseResponse<object>("تم حذف المندوب بنجاح", true, 200));
        }
    }
}
