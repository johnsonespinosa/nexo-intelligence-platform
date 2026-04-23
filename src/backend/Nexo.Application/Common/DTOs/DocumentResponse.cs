namespace Nexo.Application.Common.DTOs;

public class DocumentResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Filename { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string EmbeddingVersion { get; set; } = string.Empty;
    public int ChunkCount { get; set; }
    public DateTime CreatedAt { get; set; }
}