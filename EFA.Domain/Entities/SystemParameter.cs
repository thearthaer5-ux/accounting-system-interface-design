namespace EFA.Domain.Entities;

/// <summary>
/// جدول معاملات النظام - System_Parameters
/// يحتوي على الإعدادات والمتغيرات العامة للنظام
/// </summary>
public class SystemParameter
{
    public int ParameterId { get; set; }
    
    public string ParameterName { get; set; } = string.Empty;
    
    public string ParameterValue { get; set; } = string.Empty;
    
    public string? Category { get; set; }
    
    public string? Description { get; set; }
    
    public string? DataType { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastModifiedDate { get; set; }
    
    public string? ModifiedBy { get; set; }
}
