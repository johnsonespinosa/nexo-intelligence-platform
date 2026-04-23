namespace Nexo.Application.Common.DTOs;

public record UploadDocumentResponse(
    Guid DocumentId,
    string Status,
    int EstimatedProcessingTimeSeconds);
