using TunaEngine.Api.Models;

namespace TunaEngine.Api.Interfaces;

public interface MembersInterface
{
    Task<IReadOnlyList<Members>> GetAllMembers(CancellationToken cancellationToken);

    Task<Members?> GetByIdMember(string id, CancellationToken cancellationToken);

    Task<Members> CreateMember(
        MembersRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateMember(
        string id,
        MembersRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteMember(string id, CancellationToken cancellationToken);
}
