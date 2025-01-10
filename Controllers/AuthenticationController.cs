using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Infrastructure.JwtService;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos;
using WarehouseManagementSystem.Models.Responses;

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

        public AuthenticationController(IAsyncRepository<User> userRepository, IAsyncRepository<UserToken> userTokenRepository, IMapper mapper, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
        }

        [HttpPost]
        public async Task<BaseResponse<UserToken>> LogIn(UserloginDto user)
        {
            var UserToLogin = _userRepository.Where(u=>u.UserName == user.UserName).FirstOrDefault();
           
            if(UserToLogin == null)
            {
                return new BaseResponse<UserToken>("عذرا لا يمكن تسجيل الدخول", false, 401);
            }

            byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

            var passwprd = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            if(passwprd != UserToLogin.Password)
            {
                return new BaseResponse<UserToken>("عذرا لا يمكن تسجيل الدخول", false, 401);
            }

            var token = _userTokenRepository.Where(u => u.UserId == UserToLogin.Id).FirstOrDefault();

            if (token == null)
            {
                var Token = _jwtProvider.Generate(UserToLogin);

                var UserToken = new UserToken();

                UserToken.Token = Token;
                UserToken.UserId = UserToLogin.Id;

                await _userTokenRepository.AddAsync(UserToken);

                return new BaseResponse<UserToken>(UserToken,"تم تسجيل الدخول بنجاح", true,200);
            }
            else return new BaseResponse<UserToken>(token, "تم تسجيل الدخول بنجاح", true, 200);

        }
    }
}
