using MediatR;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Documents.Queries.GetDocumentById;

public record GetDocumentByIdQuery(Guid DocumentId, Guid UserId) : IRequest<DocumentResponse?>;
