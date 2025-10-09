using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNI_Events.Application.Dtos.Event;
using SNI_Events.Domain.Interfaces.Services;

namespace SNI_Events.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateRequestDto dto) =>
            Ok(await _service.CreateAsync(dto));

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, EventUpdateRequestDto dto) =>
            Ok(await _service.UpdateAsync(id, dto));

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
