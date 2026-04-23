using Nexo.Domain.Enums;

namespace Nexo.Domain.Entities;

public class Document : BaseEntity
{
    public Guid UserId { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string S3Key { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public string EmbeddingVersion { get; set; } = "v1";
    public int ChunkCount { get; set; }

    // Navigation properties
    public User? User { get; set; }
}
