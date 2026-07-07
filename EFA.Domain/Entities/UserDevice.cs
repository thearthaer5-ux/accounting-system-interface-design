namespace EFA.Domain.Entities;

/// <summary>
/// جدول أجهزة المستخدمين - Users_Devices
/// يتتبع الأجهزة التي يسجل دخول المستخدم عليها
/// </summary>
public class UserDevice
{
    public int DeviceId { get; set; }
    
    public int UserId { get; set; }
    
    public string DeviceName { get; set; } = string.Empty;
    
    public string? DeviceType { get; set; }
    
    public string? IPAddress { get; set; }
    
    public string? UserAgent { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastAccessDate { get; set; }

    // Foreign Key
    public virtual User User { get; set; } = null!;
}
