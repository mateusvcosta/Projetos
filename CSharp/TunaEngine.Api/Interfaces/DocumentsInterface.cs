using TunaEngine.Api.Models;

namespace TunaEngine.Api.Interfaces;

public interface DocumentsInterface
{
    Task<IReadOnlyList<Documents>> GetAll(CancellationToken cancellationToken);
    Task<Documents?> GetById(string id, CancellationToken cancellationToken);
    Task<Documents> Create(DocumentsRequest request, CancellationToken cancellationToken);
    Task<bool> Update(string id, DocumentsRequest request, CancellationToken cancellationToken);
    Task<bool> Delete(string id, CancellationToken cancellationToken);
}
