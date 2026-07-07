namespace EFA.Domain.Entities;

/// <summary>
/// جدول سجلات نشاط المستخدمين - Users_Logs
/// يسجل جميع الأنشطة والعمليات التي يقوم بها المستخدم
/// </summary>
public class UserLog
{
    public long LogId { get; set; }
    
    public int UserId { get; set; }
    
    public string ActionType { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? TableName { get; set; }
    
    public int? RecordId { get; set; }
    
    public string? OldValue { get; set; }
    
    public string? NewValue { get; set; }
    
    public string? IPAddress { get; set; }
    
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    
    public bool IsSuccessful { get; set; } = true;

    // Foreign Key
    public virtual User User { get; set; } = null!;
}
