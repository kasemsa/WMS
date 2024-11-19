﻿using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("GetAllCommissaries")]
        public async Task<IActionResult> GetAllCommissaries([FromQuery] IndexQuery query)
        {
            FilterObject filterObject = new FilterObject() { Filters = query.filters };

            var commissaries = await _commissaryRepository.GetFilterThenPagedReponseAsync(filterObject, query.page, query.perPage);

            var commissaryDtos = _mapper.Map<IEnumerable<CommissaryDto>>(commissaries);

            int Count = _commissaryRepository.WhereThenFilter(c => true, filterObject).Count();

            Pagination pagination = new Pagination(query.page, query.perPage, Count);

            return Ok(new BaseResponse<IEnumerable<CommissaryDto>>("",true,200,commissaryDtos,pagination));
        }

        [HttpGet("{commissaryId}")]
        public async Task<IActionResult> GetCommissaryById(int commissaryId)
        {
            var commissary = await _commissaryRepository.GetByIdAsync(commissaryId);
            if (commissary == null)
                return NotFound("المندوب غير موجود");

            var commissaryDto = _mapper.Map<CommissaryDto>(commissary);
            return Ok(commissaryDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommissary([FromForm] CreateCommissaryDto commissaryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (commissaryDto.Password != commissaryDto.ConfirmPassword)
                return BadRequest("كلمة السر غير متطابقة");

            var user = await _userRepository.AddAsync(_mapper.Map<User>(commissaryDto));
            var commissary = _mapper.Map<Commissary>(commissaryDto);
            commissary.UserId = user.Id;

            await _commissaryRepository.AddAsync(commissary);

            return Ok("تم إضافة المندوب بنجاح");
        }

        [HttpPut("{commissaryId}")]
        public async Task<IActionResult> UpdateCommissary([FromForm] UpdateCommissaryDto commissaryDto, int commissaryId)
        {
            var commissary = await _commissaryRepository.GetByIdAsync(commissaryId);
            if (commissary == null)
                return NotFound("المندوب غير موجود");

            _mapper.Map(commissaryDto, commissary);
            await _commissaryRepository.UpdateAsync(commissary);

            return Ok("تم تعديل المندوب بنجاح");
        }

        [HttpDelete("{commissaryId}")]
        public async Task<IActionResult> DeleteCommissary(int commissaryId)
        {
            var commissary = await _commissaryRepository.GetByIdAsync(commissaryId);
            if (commissary == null)
                return NotFound("المندوب غير موجود");

            await _commissaryRepository.DeleteAsync(commissary);

            return Ok("تم حذف المندوب بنجاح");
        }
    }
}
