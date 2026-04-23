using MediatR;

namespace Nexo.Application.Common.Events;

public class DocumentUploadedEvent : INotification
{
    public Guid DocumentId { get; }
    public string S3Key { get; }
    public Guid UserId { get; }
    public string Filename { get; }
    public DateTime OccurredAt { get; }

    public DocumentUploadedEvent(Guid documentId, string s3Key, Guid userId, string filename)
    {
        DocumentId = documentId;
        S3Key = s3Key;
        UserId = userId;
        Filename = filename;
        OccurredAt = DateTime.UtcNow;
    }
}
