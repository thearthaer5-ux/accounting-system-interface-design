namespace EFA.Domain.Entities;

/// <summary>
/// جدول ربط المجموعات بالصلاحيات
/// يحدد الصلاحيات المتاحة لكل مجموعة
/// </summary>
public class GroupPrivilege
{
    public int GroupPrivilegeId { get; set; }
    
    public int GroupId { get; set; }
    
    public int PrivilegeId { get; set; }
    
    public bool IsGranted { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public string? CreatedBy { get; set; }

    // Foreign Keys
    public virtual Group Group { get; set; } = null!;
    
    public virtual Privilege Privilege { get; set; } = null!;
}
