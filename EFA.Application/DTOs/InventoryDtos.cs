namespace EFA.Application.DTOs
{
    // Item Category DTOs
    public class ItemCategoryDto
    {
        public int ItemCategoryId { get; set; }
        public string ItemCategoryNameAr { get; set; }
        public string ItemCategoryNameEn { get; set; }
        public string ItemCategoryDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ItemCategoryCreateDto
    {
        public string ItemCategoryNameAr { get; set; }
        public string ItemCategoryNameEn { get; set; }
        public string ItemCategoryDescription { get; set; }
    }

    // Item DTOs
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }
        public int ItemCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ItemDescription { get; set; }
        public int ItemType { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal MaximumQuantity { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ItemCreateUpdateDto
    {
        public string ItemCode { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }
        public int ItemCategoryId { get; set; }
        public string ItemDescription { get; set; }
        public int ItemType { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal MaximumQuantity { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemPrice { get; set; }
    }

    // Warehouse DTOs
    public class WarehouseDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseNameAr { get; set; }
        public string WarehouseNameEn { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehousePhone { get; set; }
        public string WarehouseManager { get; set; }
        public decimal WarehouseCapacity { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
    }

    public class WarehouseCreateUpdateDto
    {
        public string WarehouseNameAr { get; set; }
        public string WarehouseNameEn { get; set; }
        public int BranchId { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehousePhone { get; set; }
        public string WarehouseManager { get; set; }
        public decimal WarehouseCapacity { get; set; }
        public bool IsMain { get; set; }
    }

    // Item Balance DTOs
    public class ItemBalanceDto
    {
        public int ItemBalanceId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime LastMovementDate { get; set; }
    }

    // Item Movement DTOs
    public class ItemMovementDto
    {
        public int ItemMovementId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int? WarehouseIdTo { get; set; }
        public string WarehouseToName { get; set; }
        public int MovementType { get; set; }
        public string MovementTypeDesc { get; set; }
        public decimal MovementQuantity { get; set; }
        public decimal MovementCost { get; set; }
        public string ReferenceDocumentType { get; set; }
        public string Notes { get; set; }
        public bool IsPosted { get; set; }
        public DateTime MovementDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ItemMovementCreateDto
    {
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }
        public int? WarehouseIdTo { get; set; }
        public int MovementType { get; set; }
        public decimal MovementQuantity { get; set; }
        public decimal MovementCost { get; set; }
        public int? ReferenceDocumentId { get; set; }
        public string ReferenceDocumentType { get; set; }
        public string Notes { get; set; }
    }

    // Inventory Count DTOs
    public class InventoryCountDto
    {
        public int InventoryCountId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string CountNumber { get; set; }
        public DateTime CountDate { get; set; }
        public string Notes { get; set; }
        public int Status { get; set; }
        public bool IsPosted { get; set; }
        public int DetailCount { get; set; }
        public decimal TotalDifference { get; set; }
    }

    public class InventoryCountDetailDto
    {
        public int InventoryCountDetailId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Difference { get; set; }
        public decimal UnitCost { get; set; }
        public decimal DifferenceCost { get; set; }
        public string Notes { get; set; }
        public bool IsAdjusted { get; set; }
    }

    public class InventoryCountCreateDto
    {
        public int WarehouseId { get; set; }
        public string Notes { get; set; }
    }

    // Item Batch DTOs
    public class ItemBatchDto
    {
        public int ItemBatchId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string BatchNumber { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal BatchQuantity { get; set; }
        public decimal BatchCost { get; set; }
        public int WarehouseId { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ItemBatchCreateDto
    {
        public int ItemId { get; set; }
        public string BatchNumber { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal BatchQuantity { get; set; }
        public decimal BatchCost { get; set; }
        public int WarehouseId { get; set; }
    }

    // Inventory Summary DTOs
    public class WarehouseInventorySummaryDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockItems { get; set; }
    }

    public class InventoryReportDto
    {
        public DateTime ReportDate { get; set; }
        public List<ItemBalanceDto> ItemBalances { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalItems { get; set; }
    }
}
