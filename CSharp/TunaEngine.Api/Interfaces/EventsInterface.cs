using TunaEngine.Api.Models;

namespace TunaEngine.Api.Interfaces;

public interface EventsInterface
{
    Task<IReadOnlyList<Events>> GetAll(CancellationToken cancellationToken);
    Task<Events?> GetById(string id, CancellationToken cancellationToken);
    Task<Events> Create(EventsRequest request, CancellationToken cancellationToken);
    Task<bool> Update(string id, EventsRequest request, CancellationToken cancellationToken);
    Task<bool> Delete(string id, CancellationToken cancellationToken);
}
