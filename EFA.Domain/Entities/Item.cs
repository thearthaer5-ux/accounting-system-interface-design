using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// Item - الأصناف
    /// تمثل المنتجات والخدمات في النظام المحاسبي
    /// </summary>
    [Table("Item")]
    public class Item
    {
        [Key]
        [Column("Item_ID")]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Item_Code")]
        public string ItemCode { get; set; }

        [Required]
        [StringLength(200)]
        [Column("Item_Name_Ar")]
        public string ItemNameAr { get; set; }

        [StringLength(200)]
        [Column("Item_Name_En")]
        public string ItemNameEn { get; set; }

        [Column("Item_Cat_ID")]
        public int ItemCategoryId { get; set; }

        [StringLength(500)]
        [Column("Item_Desc")]
        public string ItemDescription { get; set; }

        [Column("Item_Type")]
        public int ItemType { get; set; } // 1: Product, 2: Service, 3: Raw Material

        [Column("Item_Pic")]
        public byte[] ItemPicture { get; set; }

        [Column("Min_Qty")]
        public decimal MinimumQuantity { get; set; }

        [Column("Max_Qty")]
        public decimal MaximumQuantity { get; set; }

        [Column("Item_Cost")]
        public decimal ItemCost { get; set; }

        [Column("Item_Price")]
        public decimal ItemPrice { get; set; }

        [Column("Tax_Type_ID")]
        public int? TaxTypeId { get; set; }

        [Column("Account_ID")]
        public int? AccountId { get; set; }

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
        [ForeignKey("ItemCategoryId")]
        public virtual ItemCategory ItemCategory { get; set; }

        // Navigation Properties
        public virtual ICollection<ItemUnit> ItemUnits { get; set; } = new List<ItemUnit>();
        public virtual ICollection<ItemBalance> ItemBalances { get; set; } = new List<ItemBalance>();
        public virtual ICollection<ItemMovement> ItemMovements { get; set; } = new List<ItemMovement>();
        public virtual ICollection<ItemBatch> ItemBatches { get; set; } = new List<ItemBatch>();
    }
}
