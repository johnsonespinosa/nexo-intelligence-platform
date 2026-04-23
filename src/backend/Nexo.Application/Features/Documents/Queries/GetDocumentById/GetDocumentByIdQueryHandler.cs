using MediatR;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Application.Features.Documents.Queries.GetDocumentById;

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentResponse?>
{
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentByIdQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<DocumentResponse?> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document is null || document.UserId != request.UserId)
        {
            return null;
        }

        return new DocumentResponse
        {
            Id = document.Id,
            UserId = document.UserId,
            Filename = document.Filename,
            SizeBytes = document.SizeBytes,
            MimeType = document.MimeType,
            Status = document.Status.ToString(),
            EmbeddingVersion = document.EmbeddingVersion,
            ChunkCount = document.ChunkCount,
            CreatedAt = document.CreatedAt
        };
    }
}
