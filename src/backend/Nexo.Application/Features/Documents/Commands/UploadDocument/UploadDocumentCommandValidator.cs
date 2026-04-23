using FluentValidation;

namespace Nexo.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
