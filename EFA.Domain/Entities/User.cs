namespace EFA.Domain.Entities;

/// <summary>
/// جدول المستخدمين - Users
/// يحتوي على بيانات المستخدمين وتسجيل دخولهم
/// </summary>
public class User
{
    public int UserId { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public string? FullName { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginDate { get; set; }
    
    public int? GroupId { get; set; }
    
    public int? BranchId { get; set; }
    
    public DateTime? LastModifiedDate { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? ModifiedBy { get; set; }

    // Foreign Keys
    public virtual Group? Group { get; set; }
    
    public virtual Branch? Branch { get; set; }

    // Collections
    public virtual ICollection<UserDevice> Devices { get; set; } = new HashSet<UserDevice>();
    
    public virtual ICollection<UserLog> Logs { get; set; } = new HashSet<UserLog>();
    
    public virtual ICollection<Audit> AuditLogs { get; set; } = new HashSet<Audit>();
}
