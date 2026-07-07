namespace EFA.Domain.Entities;

/// <summary>
/// جدول التدقيق - Audit
/// يسجل جميع العمليات الحساسة والتغييرات الهامة في النظام
/// </summary>
public class Audit
{
    public long AuditId { get; set; }
    
    public int? UserId { get; set; }
    
    public string EntityName { get; set; } = string.Empty;
    
    public string Action { get; set; } = string.Empty;
    
    public int? EntityId { get; set; }
    
    public string? OldValues { get; set; }
    
    public string? NewValues { get; set; }
    
    public DateTime AuditDate { get; set; } = DateTime.UtcNow;
    
    public string? IPAddress { get; set; }
    
    public string? Browser { get; set; }
    
    public string? Details { get; set; }
    
    public bool IsSuccessful { get; set; } = true;

    // Foreign Key
    public virtual User? User { get; set; }
}
