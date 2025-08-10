using DataWarehouse.API.Services.Interfaces.Users;
using DataWarehouse.API.Utils.Authorization;
using DataWarehouse.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DataWarehouse.API.Controller.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var currentUser = HttpContext.User.GetUserIdentity(); 
            var profile = await _userService.RegisterAsync(dto, currentUser);
            return Ok(profile);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var auth = await _userService.LoginAsync(dto);
            return Ok(auth);
        }
    }
}