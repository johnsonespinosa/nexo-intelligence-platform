using MediatR;

namespace Nexo.Application.Features.Documents.Commands.DeleteDocument;

public record DeleteDocumentCommand(Guid DocumentId, Guid UserId) : IRequest<bool>;
