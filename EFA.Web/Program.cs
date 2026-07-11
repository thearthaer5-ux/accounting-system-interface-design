using EFA.Application.Profiles;
using EFA.Application.Services;
using EFA.Infrastructure.Data;
using EFA.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<EFADbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IPrivilegeRepository, PrivilegeRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

// Inventory Repositories
builder.Services.AddScoped<IItemCategoryRepository, ItemCategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IItemBalanceRepository, ItemBalanceRepository>();
builder.Services.AddScoped<IItemMovementRepository, ItemMovementRepository>();
builder.Services.AddScoped<IItemBatchRepository, ItemBatchRepository>();
builder.Services.AddScoped<IInventoryCountRepository, InventoryCountRepository>();

// Accounting Repositories
builder.Services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
builder.Services.AddScoped<IJournalTypeRepository, JournalTypeRepository>();
builder.Services.AddScoped<IJournalRepository, JournalRepository>();
builder.Services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
builder.Services.AddScoped<IOpeningBalanceRepository, OpeningBalanceRepository>();
builder.Services.AddScoped<IFiscalPeriodRepository, FiscalPeriodRepository>();
builder.Services.AddScoped<IAccountBalanceRepository, AccountBalanceRepository>();
builder.Services.AddScoped<ILedgerReportRepository, LedgerReportRepository>();

// Purchase Repositories
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();
builder.Services.AddScoped<IPurchaseReturnRepository, PurchaseReturnRepository>();
builder.Services.AddScoped<IVendorBalanceRepository, VendorBalanceRepository>();
builder.Services.AddScoped<IPurchasePaymentRepository, PurchasePaymentRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

// Inventory Services
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IInventoryCountService, InventoryCountService>();
builder.Services.AddScoped<IItemBatchService, ItemBatchService>();

// Accounting Services
builder.Services.AddScoped<IChartOfAccountService, ChartOfAccountService>();
builder.Services.AddScoped<IJournalService, JournalService>();
builder.Services.AddScoped<IFiscalPeriodService, FiscalPeriodService>();
builder.Services.AddScoped<IAccountBalanceService, AccountBalanceService>();
builder.Services.AddScoped<IOpeningBalanceService, OpeningBalanceService>();

// Purchase Services
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
builder.Services.AddScoped<IPurchaseReturnService, PurchaseReturnService>();
builder.Services.AddScoped<IVendorBalanceService, VendorBalanceService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication
builder.Services.AddAuthentication("DefaultScheme")
    .AddCookie("DefaultScheme", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
