using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// ItemCategory - فئات الأصناف
    /// تمثل مجموعات الأصناف في النظام
    /// </summary>
    [Table("Item_Category")]
    public class ItemCategory
    {
        [Key]
        [Column("Item_Cat_ID")]
        public int ItemCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Item_Cat_Name_Ar")]
        public string ItemCategoryNameAr { get; set; }

        [StringLength(100)]
        [Column("Item_Cat_Name_En")]
        public string ItemCategoryNameEn { get; set; }

        [StringLength(500)]
        [Column("Item_Cat_Desc")]
        public string ItemCategoryDescription { get; set; }

        [Column("Item_Cat_Pic")]
        public byte[] ItemCategoryPicture { get; set; }

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

        // Navigation Properties
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
