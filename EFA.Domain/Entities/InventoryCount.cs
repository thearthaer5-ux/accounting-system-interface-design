using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// InventoryCount - الجرد الفعلي للمخزون
    /// تسجيل نتائج عملية جرد المستودعات
    /// </summary>
    [Table("Inventory_Count")]
    public class InventoryCount
    {
        [Key]
        [Column("Inv_Count_ID")]
        public int InventoryCountId { get; set; }

        [Column("Store_ID")]
        public int WarehouseId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Count_Number")]
        public string CountNumber { get; set; }

        [Column("Count_Date")]
        public DateTime CountDate { get; set; } = DateTime.Now;

        [Column("From_Date")]
        public DateTime? FromDate { get; set; }

        [Column("To_Date")]
        public DateTime? ToDate { get; set; }

        [StringLength(500)]
        [Column("Notes")]
        public string Notes { get; set; }

        [Column("Status")]
        public int Status { get; set; } // 1: Draft, 2: In Progress, 3: Completed, 4: Approved

        [Column("Posted")]
        public bool IsPosted { get; set; } = false;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Created_By")]
        public int CreatedBy { get; set; }

        [Column("Approved_Date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("Approved_By")]
        public int? ApprovedBy { get; set; }

        // Foreign Keys
        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }

        // Navigation Properties
        public virtual ICollection<InventoryCountDetail> InventoryCountDetails { get; set; } = new List<InventoryCountDetail>();
    }
}
