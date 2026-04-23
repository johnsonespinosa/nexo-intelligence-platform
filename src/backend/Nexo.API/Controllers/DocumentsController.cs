using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Features.Documents.Commands.DeleteDocument;
using Nexo.Application.Features.Documents.Commands.UploadDocument;
using Nexo.Application.Features.Documents.Queries.GetDocumentById;
using Nexo.Application.Features.Documents.Queries.GetUserDocuments;

namespace Nexo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DocumentsController(IMediator mediator, IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadDocumentResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? metadata = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var command = new UploadDocumentCommand(file, userId, metadata);
        var result = await _mediator.Send(command);

        return Accepted(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<DocumentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDocuments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetUserDocumentsQuery(userId, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocument(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetDocumentByIdQuery(id, userId));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new DeleteDocumentCommand(id, userId));
        return result ? NoContent() : NotFound();
    }
}