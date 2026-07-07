using System;
using System.Collections.Generic;

namespace EFA.Domain.Entities
{
    /// <summary>
    /// شجرة الحسابات - الحسابات الرئيسية والفرعية
    /// </summary>
    public class ChartOfAccount
    {
        public int AccountId { get; set; }

        /// <summary>
        /// رقم الحساب (مثل 1000، 1100، 1110)
        /// </summary>
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// اسم الحساب بالعربية
        /// </summary>
        public string AccountNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم الحساب بالإنجليزية
        /// </summary>
        public string AccountNameEn { get; set; } = string.Empty;

        /// <summary>
        /// نوع الحساب (Asset, Liability, Equity, Income, Expense)
        /// </summary>
        public string AccountType { get; set; } = string.Empty;

        /// <summary>
        /// مستوى الحساب (Header, Detail)
        /// </summary>
        public string AccountLevel { get; set; } = "Detail";

        /// <summary>
        /// الحساب الأب (للحسابات الفرعية)
        /// </summary>
        public int? ParentAccountId { get; set; }

        /// <summary>
        /// علاقة الحساب الأب
        /// </summary>
        public virtual ChartOfAccount? ParentAccount { get; set; }

        /// <summary>
        /// الحسابات الفرعية
        /// </summary>
        public virtual ICollection<ChartOfAccount> SubAccounts { get; set; } = new List<ChartOfAccount>();

        /// <summary>
        /// ما إذا كان الحساب مستخدماً
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// وصف الحساب
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// الرصيد الافتتاحي
        /// </summary>
        public decimal OpeningBalance { get; set; } = 0;

        /// <summary>
        /// معرّف الفرع
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
        public virtual ICollection<AccountBalance> AccountBalances { get; set; } = new List<AccountBalance>();
    }
}
