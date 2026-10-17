using TunaEngine.Api.Models;

namespace TunaEngine.Api.Interfaces;

public interface DocumentTypesInterface
{
    Task<IReadOnlyList<DocumentType>> GetAll(CancellationToken cancellationToken);
    Task<DocumentType?> GetById(string id, CancellationToken cancellationToken);
    Task<DocumentType> Create(DocumentTypeRequest request, CancellationToken cancellationToken);
    Task<bool> Update(string id, DocumentTypeRequest request, CancellationToken cancellationToken);
    Task<bool> Delete(string id, CancellationToken cancellationToken);
}
