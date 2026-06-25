namespace Anazetesis.Core.Interfaces;

using Anazetesis.Core.Models;

public interface ITopicsService
{
    IReadOnlyList<Topic> GetAllTopics();
}
