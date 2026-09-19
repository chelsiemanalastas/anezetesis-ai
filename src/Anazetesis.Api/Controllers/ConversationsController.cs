using Anazetesis.Core.Interfaces;
using Anazetesis.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Anazetesis.Api.Controllers;

[ApiController]
[Route("api")]
[EnableRateLimiting("Default")]
public sealed class ConversationsController(
    IChatRepository chatRepo,
    ILogger<ConversationsController> logger) : ControllerBase
{
    /// <summary>Get the last 20 conversations for a browser client.</summary>
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetConversations(
        [FromQuery] string clientId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return BadRequest(new { error = "clientId is required." });

        var conversations = await chatRepo.GetConversationsAsync(clientId, 20, ct);

        var summaries = conversations.Select(c => new ConversationSummary
        {
            Id = c.Id,
            Title = c.Title,
            MessageCount = c.Messages.Count,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Ok(summaries);
    }

    /// <summary>Delete a specific conversation.</summary>
    [HttpDelete("conversations/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversation(
        Guid id,
        CancellationToken ct)
    {
        // Note: Full delete requires adding a Delete method to IChatRepository
        logger.LogWarning("Delete conversation {Id} requested but not yet implemented.", id);
        return NotFound(new { error = "Not implemented." });
    }
}