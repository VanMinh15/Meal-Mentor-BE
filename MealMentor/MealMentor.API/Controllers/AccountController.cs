using MealMentor.API.Services.AccountService;
using MealMentor.API.Services.TokenService;
using MealMentor.Core.DTOs.AccountDTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly JsonSerializerOptions _jsonOptions;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var result = await _accountService.GetAccountById(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserList([FromQuery] PaginationParams pagingParams)
        {
            var result = await _accountService.GetUserList(pagingParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _accountService.Login(loginDTO.Email, loginDTO.Password);
            return StatusCode(result.StatusCode, result);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await _accountService.RegisterUser(registerDTO);
            return StatusCode(result.StatusCode, result);
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        //{
        //    var result = await _accountService.RefreshToken(refreshToken);
        //    return StatusCode(result.StatusCode, result);
        //}

        [HttpGet("me")]
        public async Task<IActionResult> WhoAmI()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                var userRole = User.FindFirst(ClaimTypes.Role);
                var userNameClaim = User.FindFirst(ClaimTypes.Name);


                if (userIdClaim != null && userEmailClaim != null && userNameClaim != null)
                {
                    var user = await _accountService.GetAccountById(userIdClaim.Value);
                    return StatusCode(user.StatusCode, user);
                }
                else
                {
                    return Unauthorized(new { Message = "Missing user information in claims" });
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        //[AllowAnonymous]
        //[HttpGet("signin-google")]
        //public IActionResult GoogleLogin()
        //{
        //    var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

        //[Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    if (!authenticateResult.Succeeded)
        //        return BadRequest("Error signing in with Google");

        //    // Extract user info from Google
        //    var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        //    var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

        //    // Here you could create a user in your database if needed

        //    // Generate a JWT token for the user
        //    // var token = GenerateJwtToken(email, name);

        //    return Ok(new { Email = email, Name = name });
        //}
    }
}
