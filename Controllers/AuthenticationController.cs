using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Infrastructure.JwtService;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos;

namespace WarehouseManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IAsyncRepository<UserToken> _userTokenRepository;
        private readonly IMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        [HttpPost]
        public async Task<IActionResult> LogIn(UserloginDto user)
        {
            var UserToLogin = _userRepository.Where(u=>u.UserName == user.UserName).FirstOrDefault();
           
            if(UserToLogin == null)
            {
                return NotFound();
            }
            var token = _userTokenRepository.Where(u => u.UserId == UserToLogin.Id).FirstOrDefault();

            if (token == null)
            {
                var UserToken = _jwtProvider.Generate(UserToLogin);
                return Ok(UserToken);
            }
            else return Ok(token.Token);
            
        }
    }
}
