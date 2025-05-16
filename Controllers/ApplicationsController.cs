using ApplicationsChallenge.API.Models;
using ApplicationsChallenge.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApplicationsChallenge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            var applications = await _applicationService.GetAllApplicationsAsync();
            return Ok(applications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> GetApplication(int id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return Ok(application);
        }

        [HttpPost]
        public async Task<ActionResult<Application>> CreateApplication([FromBody] ApplicationCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var application = new Application
            {
                Type = dto.Type,
                Message = dto.Message,
                // Service will set the Date, Status, and UserId
            };

            var createdApplication = await _applicationService.CreateApplicationAsync(application, userId);

            return CreatedAtAction(
                nameof(GetApplication),
                new { id = createdApplication.Id },
                createdApplication);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _applicationService.UpdateApplicationStatusAsync(id, dto.Status);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var success = await _applicationService.DeleteApplicationAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

    public class ApplicationCreateDto
    {
        public required string Type { get; set; }
        public required string Message { get; set; }
    }

    public class StatusUpdateDto
    {
        public required string Status { get; set; }
    }
}
