using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// ItemBatch - دفعات الأصناف
    /// تتبع الدفعات (Batch/Lot) للأصناف مع تواريخ الصلاحية
    /// </summary>
    [Table("Item_Batch")]
    public class ItemBatch
    {
        [Key]
        [Column("Item_Batch_ID")]
        public int ItemBatchId { get; set; }

        [Column("Item_ID")]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Batch_Number")]
        public string BatchNumber { get; set; }

        [Column("Serial_Number")]
        public string SerialNumber { get; set; }

        [Column("Manufacturing_Date")]
        public DateTime? ManufacturingDate { get; set; }

        [Column("Expiry_Date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("Batch_Qty")]
        public decimal BatchQuantity { get; set; }

        [Column("Batch_Cost")]
        public decimal BatchCost { get; set; }

        [Column("Warehouse_ID")]
        public int WarehouseId { get; set; }

        [Column("Is_Available")]
        public bool IsAvailable { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Created_By")]
        public int CreatedBy { get; set; }

        // Foreign Keys
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }
    }
}
