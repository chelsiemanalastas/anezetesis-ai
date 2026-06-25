namespace Anazetesis.Core.Interfaces;

using Anazetesis.Core.Models;

public interface IAskService
{
    Task<AskResponse> AskAsync(AskRequest request, CancellationToken ct = default);
}
