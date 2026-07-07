namespace EFA.Domain.Entities;

/// <summary>
/// جدول مجموعات الصلاحيات - Group_Priv_Type
/// يحتوي على تجميعات الصلاحيات المختلفة للمستخدمين
/// </summary>
public class Group
{
    public int GroupId { get; set; }
    
    public string GroupCode { get; set; } = string.Empty;
    
    public string GroupName { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastModifiedDate { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? ModifiedBy { get; set; }

    // Collections
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    
    public virtual ICollection<GroupPrivilege> Privileges { get; set; } = new HashSet<GroupPrivilege>();
}
