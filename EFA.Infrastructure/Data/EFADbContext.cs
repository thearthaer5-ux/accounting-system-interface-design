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
    }
}
