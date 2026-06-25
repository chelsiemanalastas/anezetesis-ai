namespace Anazetesis.Api.Controllers;

using Anazetesis.Core.Interfaces;
using Anazetesis.Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public sealed class TopicsController(ITopicsService topicsService) : ControllerBase
{
    [HttpGet("topics")]
    [ProducesResponseType(typeof(IReadOnlyList<Topic>), StatusCodes.Status200OK)]
    public IActionResult GetTopics() => Ok(topicsService.GetAllTopics());
}
