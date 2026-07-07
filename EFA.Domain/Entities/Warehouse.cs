using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// Warehouse - المستودعات
    /// تمثل أماكن تخزين الأصناف في النظام
    /// </summary>
    [Table("Warehouse")]
    public class Warehouse
    {
        [Key]
        [Column("Store_ID")]
        public int WarehouseId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Store_Name_Ar")]
        public string WarehouseNameAr { get; set; }

        [StringLength(100)]
        [Column("Store_Name_En")]
        public string WarehouseNameEn { get; set; }

        [Column("Branch_ID")]
        public int BranchId { get; set; }

        [StringLength(500)]
        [Column("Store_Address")]
        public string WarehouseAddress { get; set; }

        [StringLength(50)]
        [Column("Store_Phone")]
        public string WarehousePhone { get; set; }

        [StringLength(100)]
        [Column("Store_Manager")]
        public string WarehouseManager { get; set; }

        [Column("Store_Capacity")]
        public decimal WarehouseCapacity { get; set; }

        [Column("Is_Main")]
        public bool IsMain { get; set; } = false;

        [Column("Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Created_By")]
        public int CreatedBy { get; set; }

        [Column("Modified_Date")]
        public DateTime? ModifiedDate { get; set; }

        [Column("Modified_By")]
        public int? ModifiedBy { get; set; }

        // Foreign Keys
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        // Navigation Properties
        public virtual ICollection<ItemBalance> ItemBalances { get; set; } = new List<ItemBalance>();
        public virtual ICollection<ItemMovement> ItemMovements { get; set; } = new List<ItemMovement>();
    }
}
