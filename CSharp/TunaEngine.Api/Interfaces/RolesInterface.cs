using TunaEngine.Api.Models;

namespace TunaEngine.Api.Interfaces;

public interface RolesInterface
{
    Task<IReadOnlyList<Roles>> GetAllRoles(CancellationToken cancellationToken);

    Task<Roles?> GetByIdRole(string id, CancellationToken cancellationToken);

    Task<Roles> CreateRole(
        RolesRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateRole(
        string id,
        RolesRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteRole(string id, CancellationToken cancellationToken);
}
