using MediatR;
using Microsoft.AspNetCore.Http;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Documents.Commands.UploadDocument;

public record UploadDocumentCommand(
    IFormFile File,
    Guid UserId,
    string? Metadata = null) : IRequest<UploadDocumentResponse>;
