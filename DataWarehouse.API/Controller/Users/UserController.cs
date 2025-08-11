using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Authorization;
using DataWarehouse.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataWarehouse.API.Controller.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
        private const string RefreshCookieName = "refresh_token";
        private static readonly TimeSpan RefreshTtl = TimeSpan.FromDays(7);

        public UsersController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var currentUser = HttpContext.User.GetUserIdentity();
            var profile = await _userService.RegisterAsync(dto, currentUser);
            return Ok(profile);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var auth = await _userService.LoginAsync(dto);

            if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
            {
                var cookieOptions = CookieOptionsFactory.CreateRefreshCookie(_env, RefreshTtl);
                Response.Cookies.Append(RefreshCookieName, auth.RefreshToken!, cookieOptions);
                auth.RefreshToken = null;
            }

            return Ok(auth);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var refresh = Request.Cookies[RefreshCookieName];
            if (string.IsNullOrWhiteSpace(refresh))
                return Unauthorized(new { error = "Missing refresh token." });

            var auth = await _userService.RefreshAsync(refresh);

            if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
            {
                var cookieOptions = CookieOptionsFactory.CreateRefreshCookie(_env, RefreshTtl);
                Response.Cookies.Append(RefreshCookieName, auth.RefreshToken!, cookieOptions);
                auth.RefreshToken = null;
            }

            return Ok(auth);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refresh = Request.Cookies[RefreshCookieName];
            if (!string.IsNullOrWhiteSpace(refresh))
            {
                await _userService.LogoutAsync(refresh);
            }

            var delOptions = CookieOptionsFactory.CreateRefreshCookie(_env, RefreshTtl, expireNow: true);
            Response.Cookies.Delete(RefreshCookieName, delOptions);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var user = HttpContext.User.GetUserIdentity();
            if (user == null) return Unauthorized();
            return Ok(new { user.Id, user.Username, user.Role });
        }
    }
}