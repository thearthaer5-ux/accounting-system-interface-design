using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// ItemMovement - حركات الأصناف
    /// تتبع جميع الحركات (دخول، خروج، تحويل) للأصناف في المستودعات
    /// </summary>
    [Table("Item_Mov")]
    public class ItemMovement
    {
        [Key]
        [Column("Item_Mov_ID")]
        public int ItemMovementId { get; set; }

        [Column("Item_ID")]
        public int ItemId { get; set; }

        [Column("Store_ID")]
        public int WarehouseId { get; set; }

        [Column("Store_ID_To")]
        public int? WarehouseIdTo { get; set; }

        [Column("Mov_Type")]
        public int MovementType { get; set; } // 1: In, 2: Out, 3: Transfer, 4: Inventory, 5: Return

        [Column("Mov_Qty")]
        public decimal MovementQuantity { get; set; }

        [Column("Mov_Cost")]
        public decimal MovementCost { get; set; }

        [Column("Ref_Doc_ID")]
        public int? ReferenceDocumentId { get; set; }

        [StringLength(100)]
        [Column("Ref_Doc_Type")]
        public string ReferenceDocumentType { get; set; } // PurchaseInvoice, SalesInvoice, etc.

        [StringLength(200)]
        [Column("Notes")]
        public string Notes { get; set; }

        [Column("Posted")]
        public bool IsPosted { get; set; } = false;

        [Column("Journal_ID")]
        public int? JournalId { get; set; } // Link to accounting journal

        [Column("Mov_Date")]
        public DateTime MovementDate { get; set; } = DateTime.Now;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Created_By")]
        public int CreatedBy { get; set; }

        // Foreign Keys
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("WarehouseIdTo")]
        public virtual Warehouse WarehouseTo { get; set; }
    }
}
