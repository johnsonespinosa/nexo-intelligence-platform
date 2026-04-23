using Nexo.Domain.Enums;

namespace Nexo.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public UserTier Tier { get; set; } = UserTier.Standard;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Query> Queries { get; set; } = new List<Query>();
}




