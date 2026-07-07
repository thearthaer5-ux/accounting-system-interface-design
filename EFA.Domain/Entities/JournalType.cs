using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// أنواع اليوميات المحاسبية (يومية عامة، يومية شراء، يومية مبيعات، إلخ)
    /// </summary>
    public class JournalType
    {
        public int JournalTypeId { get; set; }

        /// <summary>
        /// كود نوع اليومية
        /// </summary>
        public string JournalTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم نوع اليومية بالعربية
        /// </summary>
        public string JournalTypeNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم نوع اليومية بالإنجليزية
        /// </summary>
        public string JournalTypeNameEn { get; set; } = string.Empty;

        /// <summary>
        /// الوصف
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ما إذا كانت اليومية مستخدمة
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرّف مستخدم الإنشاء
        /// </summary>
        public int? CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
    }
}
