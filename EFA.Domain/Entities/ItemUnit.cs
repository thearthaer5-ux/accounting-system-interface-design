using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// ItemUnit - وحدات الأصناف
    /// تحديد وحدات القياس المختلفة لكل صنف
    /// </summary>
    [Table("Item_Unit")]
    public class ItemUnit
    {
        [Key]
        [Column("Item_Unit_ID")]
        public int ItemUnitId { get; set; }

        [Column("Item_ID")]
        public int ItemId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Unit_Name_Ar")]
        public string UnitNameAr { get; set; }

        [StringLength(50)]
        [Column("Unit_Name_En")]
        public string UnitNameEn { get; set; }

        [Column("Unit_Factor")]
        public decimal UnitFactor { get; set; } // معامل التحويل

        [Column("Unit_Price")]
        public decimal UnitPrice { get; set; }

        [Column("Is_Base_Unit")]
        public bool IsBaseUnit { get; set; } = false;

        [Column("Active")]
        public bool IsActive { get; set; } = true;

        [Column("Created_Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("Created_By")]
        public int CreatedBy { get; set; }

        // Foreign Keys
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}
