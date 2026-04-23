using MediatR;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Application.Features.Documents.Queries.GetUserDocuments;

public class GetUserDocumentsQueryHandler : IRequestHandler<GetUserDocumentsQuery, PaginatedResponse<DocumentResponse>>
{
    private readonly IDocumentRepository _documentRepository;

    public GetUserDocumentsQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<PaginatedResponse<DocumentResponse>> Handle(GetUserDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _documentRepository.GetByUserIdAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
        var totalCount = await _documentRepository.GetCountByUserIdAsync(request.UserId, cancellationToken);

        return new PaginatedResponse<DocumentResponse>(
            documents.Select(d => new DocumentResponse
            {
                Id = d.Id,
                UserId = d.UserId,
                Filename = d.Filename,
                SizeBytes = d.SizeBytes,
                MimeType = d.MimeType,
                Status = d.Status.ToString(),
                EmbeddingVersion = d.EmbeddingVersion,
                ChunkCount = d.ChunkCount,
                CreatedAt = d.CreatedAt
            }),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
