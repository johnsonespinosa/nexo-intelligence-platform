using Microsoft.EntityFrameworkCore;
using Nexo.Application.Common.Interfaces;
using Nexo.Domain.Entities;
using Nexo.Infrastructure.Data;

namespace Nexo.Infrastructure.Repositories;

public class DocumentRepository(ApplicationDbContext context) : IDocumentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
        => await _context.Documents
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<Document> CreateAsync(Document document, CancellationToken cancellationToken = default)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task<Document> UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        document.UpdatedAt = DateTime.UtcNow;
        _context.Documents.Update(document);
        await _context.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId, cancellationToken);

        if (document is null) return false;

        _context.Documents.Remove(document);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Documents
            .Where(d => d.UserId == userId)
            .CountAsync(cancellationToken);
}