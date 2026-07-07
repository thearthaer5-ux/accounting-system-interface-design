using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// ItemBalance - أرصدة الأصناف
    /// تتبع أرصدة الأصناف في المستودعات المختلفة
    /// </summary>
    [Table("Item_Balance")]
    public class ItemBalance
    {
        [Key]
        [Column("Item_Balance_ID")]
        public int ItemBalanceId { get; set; }

        [Column("Item_ID")]
        public int ItemId { get; set; }

        [Column("Store_ID")]
        public int WarehouseId { get; set; }

        [Column("Balance_Qty")]
        public decimal BalanceQuantity { get; set; }

        [Column("Avg_Cost")]
        public decimal AverageCost { get; set; }

        [Column("Last_Movement_Date")]
        public DateTime LastMovementDate { get; set; }

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Created_By")]
        public int CreatedBy { get; set; }

        [Column("Modified_Date")]
        public DateTime? ModifiedDate { get; set; }

        [Column("Modified_By")]
        public int? ModifiedBy { get; set; }

        // Foreign Keys
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }
    }
}
