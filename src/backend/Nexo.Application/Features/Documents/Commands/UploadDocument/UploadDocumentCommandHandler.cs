using MediatR;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Common.Events;
using Nexo.Application.Common.Interfaces;
using Nexo.Domain.Entities;
using Nexo.Domain.Enums;

namespace Nexo.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentCommandHandler(
    IDocumentRepository documentRepository,
    IStorageService storageService,
    IEventBus eventBus) : IRequestHandler<UploadDocumentCommand, UploadDocumentResponse>
{
    private readonly IDocumentRepository _documentRepository = documentRepository;
    private readonly IStorageService _storageService = storageService;
    private readonly IEventBus _eventBus = eventBus;

    public async Task<UploadDocumentResponse> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        // Validar archivo
        var allowedTypes = new[] { "application/pdf", "text/plain", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
        if (!allowedTypes.Contains(request.File.ContentType))
        {
            throw new ArgumentException($"Unsupported file type: {request.File.ContentType}");
        }

        if (request.File.Length > 50 * 1024 * 1024) // 50MB
        {
            throw new ArgumentException("File size exceeds 50MB limit");
        }

        // Generar key para storage
        var s3Key = $"documents/{request.UserId}/{Guid.NewGuid()}/{request.File.FileName}";

        // Subir a storage
        await using var stream = request.File.OpenReadStream();
        await _storageService.UploadAsync(s3Key, stream, request.File.ContentType, cancellationToken);

        // Crear documento en DB
        var document = new Document
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Filename = request.File.FileName,
            S3Key = s3Key,
            SizeBytes = request.File.Length,
            MimeType = request.File.ContentType,
            Status = DocumentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _documentRepository.CreateAsync(document, cancellationToken);

        // Publicar evento para procesamiento async
        await _eventBus.PublishAsync(new DocumentUploadedEvent(
            document.Id,
            s3Key,
            request.UserId,
            request.File.FileName
        ), cancellationToken);

        return new UploadDocumentResponse(
            document.Id,
            "pending",
            30  // Estimated 30 seconds for embedding
        );
    }
}