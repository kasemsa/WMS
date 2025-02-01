using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using WarehouseManagementSystem.Contract.BaseRepository;
using WarehouseManagementSystem.Contract.SeedServices;
using WarehouseManagementSystem.DataBase;
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
        private readonly IAsyncRepository<UserRole> _roleRepository;
        private readonly IAsyncRepository<RolePermission> _rolePermissionRepository;
        private readonly IAsyncRepository<Commissary> _commissaryRepository;
        private readonly IAsyncRepository<UserPermission> _UserPermissionRepository;
        private readonly IMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISeedService _seedServices;

        public AuthenticationController(IAsyncRepository<UserPermission> UserPermissionRepository,  IAsyncRepository<Commissary> commissaryRepository, ISeedService seedServices, IServiceProvider serviceProvider, IAsyncRepository<RolePermission> rolePermissionRepository, IAsyncRepository<UserRole> roleRepository, IAsyncRepository<User> userRepository, IAsyncRepository<UserToken> userTokenRepository, IMapper mapper, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _seedServices = seedServices;
            _serviceProvider = serviceProvider;
            _commissaryRepository = commissaryRepository;
            _UserPermissionRepository = UserPermissionRepository;
        }

        [HttpPost("LogIn")]
        public async Task<BaseResponse<AuthenticationResponse>> LogIn(UserLoginDto user)
        {
            var UserToLogin = _userRepository.Where(u=>u.UserName == user.UserName).FirstOrDefault();
           
            if(UserToLogin == null)
            {
                return new BaseResponse<AuthenticationResponse>("عذرا لا يمكن تسجيل الدخول", false, 401);
            }

            byte[] salt = new byte[16] { 41, 214, 78, 222, 28, 87, 170, 211, 217, 125, 200, 214, 185, 144, 44, 34 };

            var passwprd = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            if (passwprd != UserToLogin.Password)
            {
                return new BaseResponse<AuthenticationResponse>("عذرا لا يمكن تسجيل الدخول", false, 401);
            }
            var UserRoles = _roleRepository.Where(r => r.UserId == UserToLogin.Id).Include(r => r.Role).Select(r => r.Role).ToList();

            var Permissions = _rolePermissionRepository.Where(p => UserRoles.Contains(p.Role)).Include(p => p.Permission).Select(p => p.Permission).ToList();
            var UserPermissions = _UserPermissionRepository.Where(u => u.UserId == UserToLogin.Id).Select(u => u.Permission).ToList();

            Permissions.AddRange(UserPermissions);

            var commissary = _commissaryRepository.Where(c => c.UserId == UserToLogin.Id).First();

            var token = commissary == null
                ? _jwtProvider.Generate(UserToLogin, 0)
                : _jwtProvider.Generate(UserToLogin, commissary.Id);

            var userToken = _userTokenRepository.Where(u => u.UserId == UserToLogin.Id).FirstOrDefault();
            
            if(userToken == null)
            {
                var UserToken = new UserToken();

                UserToken.Token = token;
                UserToken.UserId = UserToLogin.Id;

                await _userTokenRepository.AddAsync(UserToken);
            }
            else
            {
                userToken.Token = token;
                await _userTokenRepository.UpdateAsync(userToken);
            }

            var Response = _mapper.Map<AuthenticationResponse>(UserToLogin);

            Response.Roles = UserRoles;
            Response.Permissions = Permissions;
            Response.Token = token;

            return new BaseResponse<AuthenticationResponse>(Response, "تم تسجيل الدخول بنجاح", true, 200);
        }

        [HttpGet("ApplySeeder", Name = "ApplySeeder")]
        public async Task<IActionResult> ApplySeeder()
        {
            await _seedServices.Initialize(_serviceProvider);

            return Ok(new BaseResponse<object>("تمت العملية بنجاح", true, 200));
        }

        [HttpGet("MigrateDatabase", Name = "MigrateDatabase")]
        public async Task<IActionResult> MigrateDatabase()
        {
            var db = _serviceProvider.GetService<WarehouseDbContext>();

            // Drop the existing database
            await db!.Database.EnsureDeletedAsync();

            // Recreate the database schema
            await db.Database.EnsureCreatedAsync();

            return Ok(new BaseResponse<object>("تمت العملية بنجاح", true, 200));

        }
    }
}
