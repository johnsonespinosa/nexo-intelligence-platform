namespace Nexo.Domain.Entities;

public class Query : BaseEntity
{
    public Guid UserId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? Response { get; set; }
    public double? Confidence { get; set; }
    public int LatencyMs { get; set; }
    public string? ModelVersion { get; set; }
    public bool CacheHit { get; set; }

    // Navigation
    public User? User { get; set; }
}
