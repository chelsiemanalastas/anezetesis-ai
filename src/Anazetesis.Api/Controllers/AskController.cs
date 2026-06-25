namespace Anazetesis.Api.Controllers;

using Anazetesis.Core.Interfaces;
using Anazetesis.Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public sealed class AskController(
    IAskService askService,
    ILogger<AskController> logger) : ControllerBase
{
    [HttpPost("ask")]
    [ProducesResponseType(typeof(AskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Ask([FromBody] AskRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new { error = "Question cannot be empty." });

        try
        {
            var response = await askService.AskAsync(request, ct);
            return Ok(response);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Upstream OpenRouter call failed.");
            return StatusCode(StatusCodes.Status502BadGateway,
                new { error = "The AI service is temporarily unavailable. Please try again." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error processing ask request.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "An unexpected error occurred. Please try again." });
        }
    }
}
