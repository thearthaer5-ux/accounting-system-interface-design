using System;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// تقارير الأستاذ العام
    /// </summary>
    public class LedgerReport
    {
        public int LedgerReportId { get; set; }

        /// <summary>
        /// معرّف الحساب
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// علاقة الحساب
        /// </summary>
        public virtual ChartOfAccount? Account { get; set; }

        /// <summary>
        /// تاريخ الحركة
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// رقم سند القيد
        /// </summary>
        public string VoucherNumber { get; set; } = string.Empty;

        /// <summary>
        /// الوصف
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// المبلغ المدين
        /// </summary>
        public decimal DebitAmount { get; set; } = 0;

        /// <summary>
        /// المبلغ الدائن
        /// </summary>
        public decimal CreditAmount { get; set; } = 0;

        /// <summary>
        /// الرصيد المتراكم
        /// </summary>
        public decimal RunningBalance { get; set; } = 0;

        /// <summary>
        /// معرّف اليومية
        /// </summary>
        public int? JournalId { get; set; }

        /// <summary>
        /// الفرع
        /// </summary>
        public int? BranchId { get; set; }

        /// <summary>
        /// الفترة المحاسبية
        /// </summary>
        public int? FiscalPeriodId { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرّف مستخدم الإنشاء
        /// </summary>
        public int? CreatedBy { get; set; }
    }
}
