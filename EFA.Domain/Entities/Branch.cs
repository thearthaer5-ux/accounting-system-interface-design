namespace EFA.Domain.Entities;

/// <summary>
/// جدول الفروع - Branch
/// يحتوي على معلومات فروع الشركة
/// </summary>
public class Branch
{
    public int BranchId { get; set; }
    
    public string BranchCode { get; set; } = string.Empty;
    
    public string BranchName { get; set; } = string.Empty;
    
    public string? Address { get; set; }
    
    public string? City { get; set; }
    
    public string? Country { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? Email { get; set; }
    
    public string? Manager { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsHeadOffice { get; set; }
    
    public int? CompanyId { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastModifiedDate { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? ModifiedBy { get; set; }

    // Collections
    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    
    public virtual ICollection<CostCenter> CostCenters { get; set; } = new HashSet<CostCenter>();
}
