using EFA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFA.Infrastructure.Data;

/// <summary>
/// سياق قاعدة البيانات الرئيسي
/// يدير جميع جداول النظام المحاسبي المتكامل
/// </summary>
public class EFADbContext : DbContext
{
    public EFADbContext(DbContextOptions<EFADbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Privilege> Privileges { get; set; } = null!;
    public DbSet<GroupPrivilege> GroupPrivileges { get; set; } = null!;
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<UserDevice> UserDevices { get; set; } = null!;
    public DbSet<UserLog> UserLogs { get; set; } = null!;
    public DbSet<Audit> Audits { get; set; } = null!;
    public DbSet<CostCenter> CostCenters { get; set; } = null!;
    public DbSet<SystemParameter> SystemParameters { get; set; } = null!;

    // Inventory DbSets
    public DbSet<ItemCategory> ItemCategories { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<ItemUnit> ItemUnits { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<ItemBalance> ItemBalances { get; set; } = null!;
    public DbSet<ItemMovement> ItemMovements { get; set; } = null!;
    public DbSet<ItemBatch> ItemBatches { get; set; } = null!;
    public DbSet<InventoryCount> InventoryCounts { get; set; } = null!;
    public DbSet<InventoryCountDetail> InventoryCountDetails { get; set; } = null!;

    // Accounting DbSets
    public DbSet<ChartOfAccount> ChartOfAccounts { get; set; } = null!;
    public DbSet<JournalType> JournalTypes { get; set; } = null!;
    public DbSet<Journal> Journals { get; set; } = null!;
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
    public DbSet<OpeningBalance> OpeningBalances { get; set; } = null!;
    public DbSet<FiscalPeriod> FiscalPeriods { get; set; } = null!;
    public DbSet<AccountBalance> AccountBalances { get; set; } = null!;
    public DbSet<LedgerReport> LedgerReports { get; set; } = null!;

    // Purchase DbSets
    public DbSet<VendorType> VendorTypes { get; set; } = null!;
    public DbSet<Vendor> Vendors { get; set; } = null!;
    public DbSet<VendorContact> VendorContacts { get; set; } = null!;
    public DbSet<Quotation> Quotations { get; set; } = null!;
    public DbSet<QuotationDetail> QuotationDetails { get; set; } = null!;
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = null!;
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; } = null!;
    public DbSet<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; } = null!;
    public DbSet<PurchaseReturn> PurchaseReturns { get; set; } = null!;
    public DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = null!;
    public DbSet<PurchasePayment> PurchasePayments { get; set; } = null!;
    public DbSet<VendorBalance> VendorBalances { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            
            entity.HasOne(e => e.Group)
                .WithMany(g => g.Users)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Group Configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId);
            entity.Property(e => e.GroupCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.GroupName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.GroupCode).IsUnique();
        });

        // Privilege Configuration
        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.PrivilegeId);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FormName).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);

            entity.HasIndex(e => e.Code).IsUnique();
        });

        // GroupPrivilege Configuration
        modelBuilder.Entity<GroupPrivilege>(entity =>
        {
            entity.HasKey(e => e.GroupPrivilegeId);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Privileges)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Privilege)
                .WithMany(p => p.GroupPrivileges)
                .HasForeignKey(e => e.PrivilegeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.GroupId, e.PrivilegeId }).IsUnique();
        });

        // Branch Configuration
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId);
            entity.Property(e => e.BranchCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BranchName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Manager).HasMaxLength(200);

            entity.HasIndex(e => e.BranchCode).IsUnique();
        });

        // Currency Configuration
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyId);
            entity.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CurrencyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Symbol).HasMaxLength(10);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);

            entity.HasIndex(e => e.CurrencyCode).IsUnique();
        });

        // UserDevice Configuration
        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.DeviceId);
            entity.Property(e => e.DeviceName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DeviceType).HasMaxLength(100);
            entity.Property(e => e.IPAddress).HasMaxLength(45);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserLog Configuration
        modelBuilder.Entity<UserLog>(entity =>
        {
            entity.HasKey(e => e.LogId);
            entity.Property(e => e.ActionType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.TableName).HasMaxLength(100);
            entity.Property(e => e.IPAddress).HasMaxLength(45);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.LogDate });
        });

        // Audit Configuration
        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.AuditId);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OldValues).HasMaxLength(4000);
            entity.Property(e => e.NewValues).HasMaxLength(4000);
            entity.Property(e => e.IPAddress).HasMaxLength(45);
            entity.Property(e => e.Browser).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.UserId, e.AuditDate });
            entity.HasIndex(e => new { e.EntityName, e.EntityId });
        });

        // CostCenter Configuration
        modelBuilder.Entity<CostCenter>(entity =>
        {
            entity.HasKey(e => e.CostCenterId);
            entity.Property(e => e.CostCenterCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CostCenterName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Department).HasMaxLength(200);
            entity.Property(e => e.Manager).HasMaxLength(200);

            entity.HasOne(e => e.Branch)
                .WithMany(b => b.CostCenters)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.CostCenterCode).IsUnique();
        });

        // SystemParameter Configuration
        modelBuilder.Entity<SystemParameter>(entity =>
        {
            entity.HasKey(e => e.ParameterId);
            entity.Property(e => e.ParameterName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ParameterValue).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DataType).HasMaxLength(50);

            entity.HasIndex(e => e.ParameterName).IsUnique();
        });

        // ItemCategory Configuration
        modelBuilder.Entity<ItemCategory>(entity =>
        {
            entity.HasKey(e => e.ItemCategoryId);
            entity.Property(e => e.ItemCategoryNameAr).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ItemCategoryNameEn).HasMaxLength(100);
            entity.Property(e => e.ItemCategoryDescription).HasMaxLength(500);
        });

        // Item Configuration
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ItemNameAr).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ItemNameEn).HasMaxLength(200);
            entity.Property(e => e.ItemCost).HasPrecision(18, 4);
            entity.Property(e => e.ItemPrice).HasPrecision(18, 4);

            entity.HasOne(e => e.ItemCategory)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.ItemCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ItemCode).IsUnique();
        });

        // ItemUnit Configuration
        modelBuilder.Entity<ItemUnit>(entity =>
        {
            entity.HasKey(e => e.ItemUnitId);
            entity.Property(e => e.UnitNameAr).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UnitNameEn).HasMaxLength(50);
            entity.Property(e => e.UnitFactor).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ItemUnits)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Warehouse Configuration
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId);
            entity.Property(e => e.WarehouseNameAr).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WarehouseNameEn).HasMaxLength(100);
            entity.Property(e => e.WarehouseCapacity).HasPrecision(18, 4);

            entity.HasOne(e => e.Branch)
                .WithMany(b => b.Warehouses)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ItemBalance Configuration
        modelBuilder.Entity<ItemBalance>(entity =>
        {
            entity.HasKey(e => e.ItemBalanceId);
            entity.Property(e => e.BalanceQuantity).HasPrecision(18, 4);
            entity.Property(e => e.AverageCost).HasPrecision(18, 6);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ItemBalances)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.ItemBalances)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ItemId, e.WarehouseId }).IsUnique();
        });

        // ItemMovement Configuration
        modelBuilder.Entity<ItemMovement>(entity =>
        {
            entity.HasKey(e => e.ItemMovementId);
            entity.Property(e => e.MovementQuantity).HasPrecision(18, 4);
            entity.Property(e => e.MovementCost).HasPrecision(18, 6);
            entity.Property(e => e.ReferenceDocumentType).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(200);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ItemMovements)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.ItemMovements)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.WarehouseTo)
                .WithMany()
                .HasForeignKey(e => e.WarehouseIdTo)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.ItemId, e.WarehouseId, e.MovementDate });
        });

        // ItemBatch Configuration
        modelBuilder.Entity<ItemBatch>(entity =>
        {
            entity.HasKey(e => e.ItemBatchId);
            entity.Property(e => e.BatchNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BatchQuantity).HasPrecision(18, 4);
            entity.Property(e => e.BatchCost).HasPrecision(18, 6);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.ItemBatches)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.ItemId, e.BatchNumber }).IsUnique();
        });

        // InventoryCount Configuration
        modelBuilder.Entity<InventoryCount>(entity =>
        {
            entity.HasKey(e => e.InventoryCountId);
            entity.Property(e => e.CountNumber).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.CountNumber).IsUnique();
        });

        // InventoryCountDetail Configuration
        modelBuilder.Entity<InventoryCountDetail>(entity =>
        {
            entity.HasKey(e => e.InventoryCountDetailId);
            entity.Property(e => e.SystemQuantity).HasPrecision(18, 4);
            entity.Property(e => e.PhysicalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.Difference).HasPrecision(18, 4);
            entity.Property(e => e.UnitCost).HasPrecision(18, 6);
            entity.Property(e => e.DifferenceCost).HasPrecision(18, 6);

            entity.HasOne(e => e.InventoryCount)
                .WithMany(ic => ic.InventoryCountDetails)
                .HasForeignKey(e => e.InventoryCountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ChartOfAccount Configuration
        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AccountNameAr).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AccountNameEn).HasMaxLength(200);
            entity.Property(e => e.AccountType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AccountLevel).HasMaxLength(50);

            entity.HasOne(e => e.ParentAccount)
                .WithMany(p => p.SubAccounts)
                .HasForeignKey(e => e.ParentAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.AccountNumber).IsUnique();
        });

        // JournalType Configuration
        modelBuilder.Entity<JournalType>(entity =>
        {
            entity.HasKey(e => e.JournalTypeId);
            entity.Property(e => e.JournalTypeCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.JournalTypeNameAr).IsRequired().HasMaxLength(100);
            entity.Property(e => e.JournalTypeNameEn).HasMaxLength(100);

            entity.HasIndex(e => e.JournalTypeCode).IsUnique();
        });

        // Journal Configuration
        modelBuilder.Entity<Journal>(entity =>
        {
            entity.HasKey(e => e.JournalId);
            entity.Property(e => e.JournalNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.JournalStatus).HasMaxLength(50);
            entity.Property(e => e.TotalDebit).HasPrecision(18, 4);
            entity.Property(e => e.TotalCredit).HasPrecision(18, 4);

            entity.HasOne(e => e.JournalType)
                .WithMany(jt => jt.Journals)
                .HasForeignKey(e => e.JournalTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.FiscalPeriod)
                .WithMany(fp => fp.Journals)
                .HasForeignKey(e => e.FiscalPeriodId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.JournalNumber).IsUnique();
        });

        // JournalEntry Configuration
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.JournalEntryId);
            entity.Property(e => e.VoucherNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DebitAmount).HasPrecision(18, 4);
            entity.Property(e => e.CreditAmount).HasPrecision(18, 4);

            entity.HasOne(e => e.Journal)
                .WithMany(j => j.JournalEntries)
                .HasForeignKey(e => e.JournalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Account)
                .WithMany(a => a.JournalEntries)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.CostCenter)
                .WithMany(cc => cc.JournalEntries)
                .HasForeignKey(e => e.CostCenterId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.VoucherNumber);
        });

        // OpeningBalance Configuration
        modelBuilder.Entity<OpeningBalance>(entity =>
        {
            entity.HasKey(e => e.OpeningBalanceId);
            entity.Property(e => e.DebitBalance).HasPrecision(18, 4);
            entity.Property(e => e.CreditBalance).HasPrecision(18, 4);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.FiscalPeriod)
                .WithMany(fp => fp.OpeningBalances)
                .HasForeignKey(e => e.FiscalPeriodId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.AccountId, e.FiscalPeriodId }).IsUnique();
        });

        // FiscalPeriod Configuration
        modelBuilder.Entity<FiscalPeriod>(entity =>
        {
            entity.HasKey(e => e.FiscalPeriodId);
            entity.Property(e => e.PeriodName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PeriodStatus).HasMaxLength(50);

            entity.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.FiscalYear, e.PeriodNumber }).IsUnique();
        });

        // AccountBalance Configuration
        modelBuilder.Entity<AccountBalance>(entity =>
        {
            entity.HasKey(e => e.AccountBalanceId);
            entity.Property(e => e.DebitBalance).HasPrecision(18, 4);
            entity.Property(e => e.CreditBalance).HasPrecision(18, 4);

            entity.HasOne(e => e.Account)
                .WithMany(a => a.AccountBalances)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.FiscalPeriod)
                .WithMany()
                .HasForeignKey(e => e.FiscalPeriodId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.AccountId, e.FiscalPeriodId }).IsUnique();
        });

        // LedgerReport Configuration
        modelBuilder.Entity<LedgerReport>(entity =>
        {
            entity.HasKey(e => e.LedgerReportId);
            entity.Property(e => e.VoucherNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DebitAmount).HasPrecision(18, 4);
            entity.Property(e => e.CreditAmount).HasPrecision(18, 4);
            entity.Property(e => e.RunningBalance).HasPrecision(18, 4);

            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.AccountId, e.TransactionDate });
        });

        // CostCenter Configuration - Update existing
        modelBuilder.Entity<CostCenter>(entity =>
        {
            entity.HasKey(e => e.CostCenterId);
            entity.Property(e => e.CostCenterCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CostCenterNameAr).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CostCenterNameEn).HasMaxLength(100);

            entity.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.CostCenterCode).IsUnique();
        });

        // VendorType Configuration
        modelBuilder.Entity<VendorType>(entity =>
        {
            entity.HasKey(e => e.VendorTypeId);
            entity.Property(e => e.VendorTypeCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.VendorTypeNameAr).IsRequired().HasMaxLength(100);
            entity.Property(e => e.VendorTypeNameEn).HasMaxLength(100);
            entity.HasIndex(e => e.VendorTypeCode).IsUnique();
        });

        // Vendor Configuration
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.VendorId);
            entity.Property(e => e.VendorCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.VendorNameAr).IsRequired().HasMaxLength(200);
            entity.Property(e => e.VendorNameEn).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 4);

            entity.HasOne(e => e.VendorType)
                .WithMany(vt => vt.Vendors)
                .HasForeignKey(e => e.VendorTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.LinkedAccount)
                .WithMany()
                .HasForeignKey(e => e.LinkedAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.VendorCode).IsUnique();
        });

        // VendorContact Configuration
        modelBuilder.Entity<VendorContact>(entity =>
        {
            entity.HasKey(e => e.VendorContactId);
            entity.Property(e => e.ContactName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);

            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.VendorContacts)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Quotation Configuration
        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.HasKey(e => e.QuotationId);
            entity.Property(e => e.QuotationNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SubTotal).HasPrecision(18, 4);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 4);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);

            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.Quotations)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.QuotationNumber).IsUnique();
        });

        // QuotationDetail Configuration
        modelBuilder.Entity<QuotationDetail>(entity =>
        {
            entity.HasKey(e => e.QuotationDetailId);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.LineTotal).HasPrecision(18, 4);

            entity.HasOne(e => e.Quotation)
                .WithMany(q => q.QuotationDetails)
                .HasForeignKey(e => e.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PurchaseOrder Configuration
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderId);
            entity.Property(e => e.PurchaseOrderNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SubTotal).HasPrecision(18, 4);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 4);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 4);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);
            entity.Property(e => e.ReceivedQuantityPercent).HasPrecision(5, 2);

            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.PurchaseOrders)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.PurchaseOrderNumber).IsUnique();
        });

        // PurchaseOrderDetail Configuration
        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderDetailId);
            entity.Property(e => e.OrderedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.LineTotal).HasPrecision(18, 4);

            entity.HasOne(e => e.PurchaseOrder)
                .WithMany(po => po.PurchaseOrderDetails)
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PurchaseInvoice Configuration
        modelBuilder.Entity<PurchaseInvoice>(entity =>
        {
            entity.HasKey(e => e.PurchaseInvoiceId);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.VendorInvoiceNumber).HasMaxLength(100);
            entity.Property(e => e.SubTotal).HasPrecision(18, 4);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 4);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 4);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 4);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);

            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.PurchaseInvoices)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.PurchaseOrder)
                .WithMany()
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
        });

        // PurchaseInvoiceDetail Configuration
        modelBuilder.Entity<PurchaseInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseInvoiceDetailId);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.LineTotal).HasPrecision(18, 4);

            entity.HasOne(e => e.PurchaseInvoice)
                .WithMany(pi => pi.PurchaseInvoiceDetails)
                .HasForeignKey(e => e.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PurchaseReturn Configuration
        modelBuilder.Entity<PurchaseReturn>(entity =>
        {
            entity.HasKey(e => e.PurchaseReturnId);
            entity.Property(e => e.ReturnNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SubTotal).HasPrecision(18, 4);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 4);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
            entity.Property(e => e.CreditNoteAmount).HasPrecision(18, 4);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);

            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.PurchaseReturns)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.PurchaseInvoice)
                .WithMany()
                .HasForeignKey(e => e.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.ReturnNumber).IsUnique();
        });

        // PurchaseReturnDetail Configuration
        modelBuilder.Entity<PurchaseReturnDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseReturnDetailId);
            entity.Property(e => e.ReturnedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.LineTotal).HasPrecision(18, 4);

            entity.HasOne(e => e.PurchaseReturn)
                .WithMany(pr => pr.PurchaseReturnDetails)
                .HasForeignKey(e => e.PurchaseReturnId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Item)
                .WithMany()
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PurchasePayment Configuration
        modelBuilder.Entity<PurchasePayment>(entity =>
        {
            entity.HasKey(e => e.PurchasePaymentId);
            entity.Property(e => e.PaymentNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PaymentAmount).HasPrecision(18, 4);
            entity.Property(e => e.ExchangeRate).HasPrecision(18, 6);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);

            entity.HasOne(e => e.Vendor)
                .WithMany()
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.PurchaseInvoice)
                .WithMany(pi => pi.PurchasePayments)
                .HasForeignKey(e => e.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.PaymentNumber).IsUnique();
        });

        // VendorBalance Configuration
        modelBuilder.Entity<VendorBalance>(entity =>
        {
            entity.HasKey(e => e.VendorBalanceId);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 4);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 4);
            entity.Property(e => e.BalanceAmount).HasPrecision(18, 4);

            entity.HasOne(e => e.Vendor)
                .WithMany(v => v.VendorBalances)
                .HasForeignKey(e => e.VendorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => new { e.VendorId, e.CurrencyId }).IsUnique();
        });
    }
}
