using Nexo.Domain.Entities;

namespace Nexo.Application.Common.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Document> CreateAsync(Document document, CancellationToken cancellationToken = default);
    Task<Document> UpdateAsync(Document document, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
