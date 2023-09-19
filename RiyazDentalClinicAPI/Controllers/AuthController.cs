using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RiyazDentalClinicAPI.Data;
using RiyazDentalClinicAPI.Models.Dto;
using RiyazDentalClinicAPI.Models;
using RiyazDentalClinicAPI.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RiyazDentalClinicAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secret;
        public AuthController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _db = db;
            _response = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;
            secret = configuration.GetValue<string>("ApiSettings:Secret");
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto registerRequestDto)
        {
            ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == registerRequestDto.Username.ToLower());
            if (applicationUser != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }
            ApplicationUser newUser = new()
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username,
                NormalizedEmail = registerRequestDto.Username.ToUpper(),
                Name = registerRequestDto.Name,
            };
            var result = await _userManager.CreateAsync(newUser, registerRequestDto.Password);    //This time we used CreateAsync instead of Add since this record goes under identity table and there are some security features for password and all
            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                }
                await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            else
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while Registering");
                return BadRequest(_response);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto loginRequestDto)
        {
            ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.Username.ToLower());
            if (applicationUser == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username is incorrect");
                return BadRequest(_response);
            }
            bool isValid = await _userManager.CheckPasswordAsync(applicationUser, loginRequestDto.Password);
            if (isValid == false)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Password is incorrect");
                return BadRequest(_response);
            }
            //Creating JWT token. This token is created when authentication is successful (Unlearnable code)
            var roles = await _userManager.GetRolesAsync(applicationUser);
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(secret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {   //On decoding the token from jwt.io the below properties will be visible
                    new Claim("fullname",applicationUser.Name),
                    new Claim("id", applicationUser.Id.ToString()),
                    new Claim(ClaimTypes.Email, applicationUser.Email),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(365),   //No activity till 7 days then expire
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            //Token created
            LoginResponseDto loginResponseDto = new()
            {
                Email = applicationUser.Email,
                Token = tokenHandler.WriteToken(token)
            };
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponseDto;
            return Ok(_response);
        }
    }
}
