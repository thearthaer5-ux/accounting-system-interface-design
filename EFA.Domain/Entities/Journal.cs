using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// اليوميات المحاسبية
    /// </summary>
    public class Journal
    {
        public int JournalId { get; set; }

        /// <summary>
        /// رقم اليومية
        /// </summary>
        public string JournalNumber { get; set; } = string.Empty;

        /// <summary>
        /// نوع اليومية
        /// </summary>
        public int JournalTypeId { get; set; }

        /// <summary>
        /// علاقة نوع اليومية
        /// </summary>
        public virtual JournalType? JournalType { get; set; }

        /// <summary>
        /// تاريخ اليومية
        /// </summary>
        public DateTime JournalDate { get; set; }

        /// <summary>
        /// تاريخ الترحيل
        /// </summary>
        public DateTime? PostingDate { get; set; }

        /// <summary>
        /// الفترة المحاسبية
        /// </summary>
        public int? FiscalPeriodId { get; set; }

        /// <summary>
        /// علاقة الفترة المحاسبية
        /// </summary>
        public virtual FiscalPeriod? FiscalPeriod { get; set; }

        /// <summary>
        /// الفرع
        /// </summary>
        public int? BranchId { get; set; }

        /// <summary>
        /// علاقة الفرع
        /// </summary>
        public virtual Branch? Branch { get; set; }

        /// <summary>
        /// الحالة (Draft, Posted, Reversed)
        /// </summary>
        public string JournalStatus { get; set; } = "Draft";

        /// <summary>
        /// الوصف
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// إجمالي المدين
        /// </summary>
        public decimal TotalDebit { get; set; } = 0;

        /// <summary>
        /// إجمالي الدائن
        /// </summary>
        public decimal TotalCredit { get; set; } = 0;

        /// <summary>
        /// المستند المرجعي
        /// </summary>
        public string? ReferenceDocument { get; set; }

        /// <summary>
        /// معرّف المستند الأصلي (من مبيعات أو شراء)
        /// </summary>
        public int? SourceDocumentId { get; set; }

        /// <summary>
        /// نوع المستند الأصلي
        /// </summary>
        public string? SourceDocumentType { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرّف مستخدم الإنشاء
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// تاريخ التعديل
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// معرّف مستخدم التعديل
        /// </summary>
        public int? ModifiedBy { get; set; }

        // Navigation properties
        public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
