using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// InventoryCountDetail - تفاصيل الجرد الفعلي
    /// تسجيل تفاصيل كل صنف في عملية الجرد
    /// </summary>
    [Table("Inventory_Count_Detail")]
    public class InventoryCountDetail
    {
        [Key]
        [Column("Inv_Count_Detail_ID")]
        public int InventoryCountDetailId { get; set; }

        [Column("Inv_Count_ID")]
        public int InventoryCountId { get; set; }

        [Column("Item_ID")]
        public int ItemId { get; set; }

        [Column("System_Qty")]
        public decimal SystemQuantity { get; set; }

        [Column("Physical_Qty")]
        public decimal PhysicalQuantity { get; set; }

        [Column("Difference")]
        public decimal Difference { get; set; }

        [Column("Unit_Cost")]
        public decimal UnitCost { get; set; }

        [Column("Difference_Cost")]
        public decimal DifferenceCost { get; set; }

        [StringLength(500)]
        [Column("Notes")]
        public string Notes { get; set; }

        [Column("Adjusted")]
        public bool IsAdjusted { get; set; } = false;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Keys
        [ForeignKey("InventoryCountId")]
        public virtual InventoryCount InventoryCount { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}
