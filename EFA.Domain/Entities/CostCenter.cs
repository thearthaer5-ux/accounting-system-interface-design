namespace EFA.Domain.Entities;

/// <summary>
/// جدول مراكز التكلفة - Cost_Center
/// يحتوي على تقسيمات التكاليف حسب الفروع والأقسام
/// </summary>
public class CostCenter
{
    public int CostCenterId { get; set; }
    
    public string CostCenterCode { get; set; } = string.Empty;
    
    public string CostCenterName { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public int? BranchId { get; set; }
    
    public string? Department { get; set; }
    
    public string? Manager { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastModifiedDate { get; set; }

    // Foreign Key
    public virtual Branch? Branch { get; set; }
}
