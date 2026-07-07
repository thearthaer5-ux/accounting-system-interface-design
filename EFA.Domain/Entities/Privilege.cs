namespace EFA.Domain.Entities;

/// <summary>
/// جدول الصلاحيات - Privilege
/// يحتوي على تفاصيل الصلاحيات المتاحة في النظام
/// </summary>
public class Privilege
{
    public int PrivilegeId { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? FormName { get; set; }
    
    public bool CanAdd { get; set; }
    
    public bool CanEdit { get; set; }
    
    public bool CanDelete { get; set; }
    
    public bool CanView { get; set; }
    
    public bool CanPrint { get; set; }
    
    public bool CanExport { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int DisplayOrder { get; set; }
    
    public string? Category { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastModifiedDate { get; set; }

    // Collections
    public virtual ICollection<GroupPrivilege> GroupPrivileges { get; set; } = new HashSet<GroupPrivilege>();
}
