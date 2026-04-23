using MediatR;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Documents.Queries.GetUserDocuments;

public record GetUserDocumentsQuery(Guid UserId, int Page, int PageSize) : IRequest<PaginatedResponse<DocumentResponse>>;
