namespace EFA.Domain.Entities;

/// <summary>
/// جدول العملات - Currency
/// يحتوي على أنواع العملات وسعر الصرف
/// </summary>
public class Currency
{
    public int CurrencyId { get; set; }
    
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string CurrencyName { get; set; } = string.Empty;
    
    public string? Symbol { get; set; }
    
    public bool IsDefault { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public decimal ExchangeRate { get; set; } = 1m;
    
    public int DecimalPlaces { get; set; } = 2;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastModifiedDate { get; set; }
    
    public DateTime? LastRateUpdate { get; set; }
    
    public string? UpdatedBy { get; set; }
}
