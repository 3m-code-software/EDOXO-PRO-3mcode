using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Dashboard;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IGenericRepository<Sale> _saleRepo;
    private readonly IGenericRepository<Purchase> _purchaseRepo;
    private readonly IGenericRepository<Expense> _expenseRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<Customer> _customerRepo;
    private readonly IGenericRepository<Supplier> _supplierRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IGenericRepository<Sale> saleRepo,
        IGenericRepository<Purchase> purchaseRepo,
        IGenericRepository<Expense> expenseRepo,
        IGenericRepository<Product> productRepo,
        IGenericRepository<Customer> customerRepo,
        IGenericRepository<Supplier> supplierRepo,
        IMapper mapper,
        ILogger<DashboardService> logger)
    {
        _saleRepo = saleRepo;
        _purchaseRepo = purchaseRepo;
        _expenseRepo = expenseRepo;
        _productRepo = productRepo;
        _customerRepo = customerRepo;
        _supplierRepo = supplierRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync()
    {
        try
        {
            var sales = await _saleRepo.FindAsync(s => !s.IsDeleted && s.Status == SaleStatus.Confirmed);
            var purchases = await _purchaseRepo.FindAsync(p => !p.IsDeleted && p.Status == PurchaseStatus.Received);
            var expenses = await _expenseRepo.FindAsync(e => !e.IsDeleted);
            var products = await _productRepo.FindAsync(p => !p.IsDeleted);
            var customers = await _customerRepo.FindAsync(c => !c.IsDeleted);
            var suppliers = await _supplierRepo.FindAsync(s => !s.IsDeleted);

            var summary = new DashboardSummaryDto
            {
                TotalSales = sales.Sum(s => s.Total),
                TotalSalesCount = sales.Count,
                TotalPurchases = purchases.Sum(p => p.Total),
                TotalPurchasesCount = purchases.Count,
                TotalExpenses = expenses.Sum(e => e.Amount),
                TotalExpensesCount = expenses.Count,
                TotalProducts = products.Count,
                TotalCustomers = customers.Count,
                TotalSuppliers = suppliers.Count,
                NetProfit = sales.Sum(s => s.Total) - purchases.Sum(p => p.Total) - expenses.Sum(e => e.Amount),
                SalesCount = sales.Count,
                PurchaseCount = purchases.Count,
                CustomerCount = customers.Count,
                SupplierCount = suppliers.Count,
                ProductCount = products.Count,
                LowStockCount = products.Count(p => p.CurrentStock <= p.MinStock)
            };

            return ApiResponse<DashboardSummaryDto>.Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب ملخص لوحة التحكم");
            return ApiResponse<DashboardSummaryDto>.Fail("حدث خطأ أثناء جلب الملخص");
        }
    }

    public async Task<ApiResponse<SalesChartDto>> GetSalesChartAsync(int days)
    {
        try
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);
            var sales = await _saleRepo.FindAsync(s => !s.IsDeleted && s.Date >= fromDate);
            var purchases = await _purchaseRepo.FindAsync(p => !p.IsDeleted && p.Date >= fromDate);

            var salesData = sales
                .GroupBy(s => s.Date.Date)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = g.Sum(s => s.Total)
                })
                .OrderBy(c => c.Label)
                .ToList();

            var purchasesData = purchases
                .GroupBy(p => p.Date.Date)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = g.Sum(p => p.Total)
                })
                .OrderBy(c => c.Label)
                .ToList();

            var result = new SalesChartDto
            {
                SalesData = salesData,
                PurchasesData = purchasesData
            };

            return ApiResponse<SalesChartDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب بيانات الرسم البياني");
            return ApiResponse<SalesChartDto>.Fail("حدث خطأ أثناء جلب بيانات الرسم البياني");
        }
    }

    public async Task<ApiResponse<List<AnnualChartDataDto>>> GetAnnualChartAsync(int year)
    {
        try
        {
            var sales = await _saleRepo.FindAsync(s => !s.IsDeleted && s.Date.Year == year);
            var purchases = await _purchaseRepo.FindAsync(p => !p.IsDeleted && p.Date.Year == year);

            var chartData = Enumerable.Range(1, 12).Select(m =>
            {
                var monthSales = sales.Where(s => s.Date.Month == m).Sum(s => s.Total);
                var monthPurchases = purchases.Where(p => p.Date.Month == m).Sum(p => p.Total);
                return new AnnualChartDataDto
                {
                    Month = new DateTime(year, m, 1).ToString("MMM"),
                    SalesAmount = monthSales,
                    PurchaseAmount = monthPurchases,
                    Profit = monthSales - monthPurchases
                };
            }).ToList();

            return ApiResponse<List<AnnualChartDataDto>>.Ok(chartData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب بيانات الرسم البياني السنوي");
            return ApiResponse<List<AnnualChartDataDto>>.Fail("حدث خطأ أثناء جلب البيانات السنوية");
        }
    }

    public async Task<ApiResponse<List<RecentOrderDto>>> GetRecentOrdersAsync(int count)
    {
        try
        {
            var sales = await _saleRepo.FindAsync(s => !s.IsDeleted);
            var list = sales
                .OrderByDescending(s => s.Date)
                .Take(count)
                .Select(s => new RecentOrderDto
                {
                    Id = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    CustomerName = s.Customer.Name,
                    Total = s.Total,
                    Status = s.Status.ToString(),
                    PaymentStatus = s.PaymentStatus.ToString(),
                    Date = s.Date
                })
                .ToList();

            return ApiResponse<List<RecentOrderDto>>.Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب آخر الطلبات");
            return ApiResponse<List<RecentOrderDto>>.Fail("حدث خطأ أثناء جلب آخر الطلبات");
        }
    }

    public async Task<ApiResponse<List<PendingShipmentDto>>> GetPendingShipmentsAsync()
    {
        try
        {
            var sales = await _saleRepo.FindAsync(s => !s.IsDeleted && s.ShippingStatus == ShippingStatus.Pending);
            var list = sales.Select(s => new PendingShipmentDto
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                CustomerName = s.Customer.Name,
                Total = s.Total,
                Date = s.Date
            }).ToList();

            return ApiResponse<List<PendingShipmentDto>>.Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الشحنات المعلقة");
            return ApiResponse<List<PendingShipmentDto>>.Fail("حدث خطأ أثناء جلب الشحنات المعلقة");
        }
    }

    public async Task<ApiResponse<List<InventoryAlertDto>>> GetInventoryAlertsAsync()
    {
        try
        {
            var products = await _productRepo.FindAsync(p => !p.IsDeleted && p.CurrentStock <= p.MinStock);
            var list = products.Select(p => new InventoryAlertDto
            {
                Id = p.Id,
                ProductName = p.Name,
                SKU = p.SKU,
                CurrentStock = p.CurrentStock,
                MinStock = p.MinStock
            }).ToList();

            return ApiResponse<List<InventoryAlertDto>>.Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب تنبيهات المخزون");
            return ApiResponse<List<InventoryAlertDto>>.Fail("حدث خطأ أثناء جلب تنبيهات المخزون");
        }
    }

    public async Task<ApiResponse<List<PaymentDueDto>>> GetPaymentDuesAsync()
    {
        try
        {
            var sales = await _saleRepo.FindAsync(s => !s.IsDeleted &&
                (s.PaymentStatus == PaymentStatus.Unpaid || s.PaymentStatus == PaymentStatus.Partial));

            var list = sales.Select(s => new PaymentDueDto
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                CustomerName = s.Customer.Name,
                Total = s.Total,
                PaidAmount = s.PaidAmount,
                DueAmount = s.Total - s.PaidAmount,
                PaymentStatus = s.PaymentStatus.ToString(),
                Date = s.Date
            }).ToList();

            return ApiResponse<List<PaymentDueDto>>.Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المدفوعات المستحقة");
            return ApiResponse<List<PaymentDueDto>>.Fail("حدث خطأ أثناء جلب المدفوعات المستحقة");
        }
    }
}
