using System;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// سند القيد - تفاصيل قيود اليومية
    /// </summary>
    public class JournalEntry
    {
        public int JournalEntryId { get; set; }

        /// <summary>
        /// رقم سند القيد
        /// </summary>
        public string VoucherNumber { get; set; } = string.Empty;

        /// <summary>
        /// معرّف اليومية
        /// </summary>
        public int JournalId { get; set; }

        /// <summary>
        /// علاقة اليومية
        /// </summary>
        public virtual Journal? Journal { get; set; }

        /// <summary>
        /// معرّف الحساب
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// علاقة الحساب
        /// </summary>
        public virtual ChartOfAccount? Account { get; set; }

        /// <summary>
        /// مركز التكلفة (اختياري)
        /// </summary>
        public int? CostCenterId { get; set; }

        /// <summary>
        /// علاقة مركز التكلفة
        /// </summary>
        public virtual CostCenter? CostCenter { get; set; }

        /// <summary>
        /// المبلغ المدين
        /// </summary>
        public decimal DebitAmount { get; set; } = 0;

        /// <summary>
        /// المبلغ الدائن
        /// </summary>
        public decimal CreditAmount { get; set; } = 0;

        /// <summary>
        /// الوصف
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// الفرع
        /// </summary>
        public int? BranchId { get; set; }

        /// <summary>
        /// رقم الصف في القيد
        /// </summary>
        public int? LineNumber { get; set; }

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
    }
}
