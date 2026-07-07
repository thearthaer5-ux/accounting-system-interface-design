using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// الأرصدة الافتتاحية للحسابات
    /// </summary>
    public class OpeningBalance
    {
        public int OpeningBalanceId { get; set; }

        /// <summary>
        /// معرّف الحساب
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// علاقة الحساب
        /// </summary>
        public virtual ChartOfAccount? Account { get; set; }

        /// <summary>
        /// الفترة المحاسبية
        /// </summary>
        public int FiscalPeriodId { get; set; }

        /// <summary>
        /// علاقة الفترة
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
        /// الرصيد المدين
        /// </summary>
        public decimal DebitBalance { get; set; } = 0;

        /// <summary>
        /// الرصيد الدائن
        /// </summary>
        public decimal CreditBalance { get; set; } = 0;

        /// <summary>
        /// الحالة (Draft, Posted)
        /// </summary>
        public string Status { get; set; } = "Draft";

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// معرّف مستخدم الإنشاء
        /// </summary>
        public int? CreatedBy { get; set; }
    }

    /// <summary>
    /// مراكز التكاليف
    /// </summary>
    public class CostCenter
    {
        public int CostCenterId { get; set; }

        /// <summary>
        /// كود مركز التكلفة
        /// </summary>
        public string CostCenterCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم مركز التكلفة بالعربية
        /// </summary>
        public string CostCenterNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم مركز التكلفة بالإنجليزية
        /// </summary>
        public string CostCenterNameEn { get; set; } = string.Empty;

        /// <summary>
        /// الوصف
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// الفرع
        /// </summary>
        public int? BranchId { get; set; }

        /// <summary>
        /// علاقة الفرع
        /// </summary>
        public virtual Branch? Branch { get; set; }

        /// <summary>
        /// ما إذا كان مستخدماً
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
        public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }

    /// <summary>
    /// الفترات المحاسبية
    /// </summary>
    public class FiscalPeriod
    {
        public int FiscalPeriodId { get; set; }

        /// <summary>
        /// اسم الفترة
        /// </summary>
        public string PeriodName { get; set; } = string.Empty;

        /// <summary>
        /// السنة المالية
        /// </summary>
        public int FiscalYear { get; set; }

        /// <summary>
        /// رقم الفترة (1-12)
        /// </summary>
        public int PeriodNumber { get; set; }

        /// <summary>
        /// تاريخ البداية
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ النهاية
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// الحالة (Open, Closed, Locked)
        /// </summary>
        public string PeriodStatus { get; set; } = "Open";

        /// <summary>
        /// الفرع
        /// </summary>
        public int? BranchId { get; set; }

        /// <summary>
        /// علاقة الفرع
        /// </summary>
        public virtual Branch? Branch { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
        public virtual ICollection<OpeningBalance> OpeningBalances { get; set; } = new List<OpeningBalance>();
    }

    /// <summary>
    /// أرصدة الحسابات
    /// </summary>
    public class AccountBalance
    {
        public int AccountBalanceId { get; set; }

        /// <summary>
        /// معرّف الحساب
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// علاقة الحساب
        /// </summary>
        public virtual ChartOfAccount? Account { get; set; }

        /// <summary>
        /// الفترة المحاسبية
        /// </summary>
        public int? FiscalPeriodId { get; set; }

        /// <summary>
        /// علاقة الفترة
        /// </summary>
        public virtual FiscalPeriod? FiscalPeriod { get; set; }

        /// <summary>
        /// الفرع
        /// </summary>
        public int? BranchId { get; set; }

        /// <summary>
        /// الرصيد المدين
        /// </summary>
        public decimal DebitBalance { get; set; } = 0;

        /// <summary>
        /// الرصيد الدائن
        /// </summary>
        public decimal CreditBalance { get; set; } = 0;

        /// <summary>
        /// تاريخ آخر تحديث
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
