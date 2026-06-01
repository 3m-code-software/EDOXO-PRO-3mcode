using Microsoft.Extensions.DependencyInjection;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Application.Services;

namespace EdoxoPro.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerGroupService, CustomerGroupService>();
        services.AddScoped<IDelegateService, DelegateService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductCategoryService, ProductCategoryService>();
        services.AddScoped<IProductBrandService, ProductBrandService>();
        services.AddScoped<IProductUnitService, ProductUnitService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<ISaleReturnService, SaleReturnService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IPurchaseReturnService, PurchaseReturnService>();
        services.AddScoped<IStockTransferService, StockTransferService>();
        services.AddScoped<IDamagedStockService, DamagedStockService>();
        services.AddScoped<IInventoryAuditService, InventoryAuditService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
        services.AddScoped<ICheckService, CheckService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ICompanySettingService, CompanySettingService>();
        services.AddScoped<IInvoiceSettingService, InvoiceSettingService>();
        services.AddScoped<IBarcodeSettingService, BarcodeSettingService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        return services;
    }
}
