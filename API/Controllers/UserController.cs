using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNI_Events.Application.Dtos.User;
using SNI_Events.Domain.Interfaces.Services;

namespace SNI_Events.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateRequestDto request)
        {
            var userDto = await _userService.CreateAsync(request.Name, request.Email, request.Password, request.PhoneNumber, request.CPF);
            return CreatedAtAction(nameof(GetById), new { id = 0 /* id not exposed in DTO */ }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UserUpdateRequestDto request)
        {
            var userDto = await _userService.UpdateAsync(id, request.Name, request.Email, request.PhoneNumber, request.CPF);
            return Ok(userDto);
        }

        [HttpPut("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(long id, [FromBody] UserChangePasswordRequestDto request)
        {
            var userDto = await _userService.ChangePasswordAsync(id, request.Password);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var userDto = await _userService.GetByIdAsync(id);
            if (userDto == null) return NotFound();
            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter) => Ok(await _userService.GetAllAsync(filter));

        [HttpGet("paged")]
        //[Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetPaged([FromQuery] UserFilterDto filter)
        {
            var result = await _userService.GetPagedAsync(filter);
            return Ok(result);
        }
    }
}
