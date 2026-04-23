using MediatR;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Application.Features.Documents.Commands.DeleteDocument;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IStorageService _storageService;

    public DeleteDocumentCommandHandler(IDocumentRepository documentRepository, IStorageService storageService)
    {
        _documentRepository = documentRepository;
        _storageService = storageService;
    }

    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document is null || document.UserId != request.UserId)
        {
            return false;
        }

        // Eliminar archivo del storage
        await _storageService.DeleteAsync(document.S3Key, cancellationToken);

        // Eliminar registro de la base de datos
        return await _documentRepository.DeleteAsync(request.DocumentId, request.UserId, cancellationToken);
    }
}
