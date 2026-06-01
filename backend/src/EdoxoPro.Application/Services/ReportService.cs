using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Reports;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class ReportService : IReportService
{
    private readonly IGenericRepository<Sale> _saleRepo;
    private readonly IGenericRepository<SaleItem> _saleItemRepo;
    private readonly IGenericRepository<Purchase> _purchaseRepo;
    private readonly IGenericRepository<PurchaseItem> _purchaseItemRepo;
    private readonly IGenericRepository<Expense> _expenseRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IGenericRepository<Sale> saleRepo,
        IGenericRepository<SaleItem> saleItemRepo,
        IGenericRepository<Purchase> purchaseRepo,
        IGenericRepository<PurchaseItem> purchaseItemRepo,
        IGenericRepository<Expense> expenseRepo,
        IGenericRepository<Product> productRepo,
        IMapper mapper,
        ILogger<ReportService> logger)
    {
        _saleRepo = saleRepo;
        _saleItemRepo = saleItemRepo;
        _purchaseRepo = purchaseRepo;
        _purchaseItemRepo = purchaseItemRepo;
        _expenseRepo = expenseRepo;
        _productRepo = productRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<ProfitLossDto>> GetProfitLossAsync(ReportRequest request)
    {
        try
        {
            var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-1);
            var toDate = request.ToDate ?? DateTime.UtcNow;

            var sales = await _saleRepo.FindAsync(s =>
                !s.IsDeleted && s.Status == SaleStatus.Confirmed && s.Date >= fromDate && s.Date <= toDate);

            var purchases = await _purchaseRepo.FindAsync(p =>
                !p.IsDeleted && p.Status == PurchaseStatus.Received && p.Date >= fromDate && p.Date <= toDate);

            var expenses = await _expenseRepo.FindAsync(e =>
                !e.IsDeleted && e.Date >= fromDate && e.Date <= toDate);

            var cogs = purchases.Sum(p => p.Total);

            var result = new ProfitLossDto
            {
                TotalRevenue = sales.Sum(s => s.Total),
                TotalCogs = cogs,
                TotalExpenses = expenses.Sum(e => e.Amount),
                FromDate = fromDate,
                ToDate = toDate
            };

            return ApiResponse<ProfitLossDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تقرير الأرباح والخسائر");
            return ApiResponse<ProfitLossDto>.Fail("حدث خطأ أثناء إنشاء التقرير");
        }
    }

    public async Task<ApiResponse<SalesReportDto>> GetSalesReportAsync(ReportRequest request)
    {
        try
        {
            var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-1);
            var toDate = request.ToDate ?? DateTime.UtcNow;

            var sales = await _saleRepo.FindAsync(s =>
                !s.IsDeleted && s.Date >= fromDate && s.Date <= toDate);

            var result = new SalesReportDto
            {
                TotalSales = sales.Where(s => s.Status == SaleStatus.Confirmed).Sum(s => s.Total),
                TotalOrders = sales.Count(s => s.Status == SaleStatus.Confirmed),
                TotalDiscounts = sales.Sum(s => s.Discount),
                TotalTax = sales.Sum(s => s.Tax),
                ConfirmedOrders = sales.Count(s => s.Status == SaleStatus.Confirmed),
                DraftOrders = sales.Count(s => s.Status == SaleStatus.Draft),
                CancelledOrders = sales.Count(s => s.Status == SaleStatus.Cancelled),
                FromDate = fromDate,
                ToDate = toDate
            };

            return ApiResponse<SalesReportDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تقرير المبيعات");
            return ApiResponse<SalesReportDto>.Fail("حدث خطأ أثناء إنشاء تقرير المبيعات");
        }
    }

    public async Task<ApiResponse<InventoryReportDto>> GetInventoryReportAsync(ReportRequest request)
    {
        try
        {
            var products = await _productRepo.FindAsync(p => !p.IsDeleted);

            var result = new InventoryReportDto
            {
                TotalProducts = products.Count,
                LowStockProducts = products.Count(p => p.CurrentStock <= p.MinStock && p.CurrentStock > 0),
                OutOfStockProducts = products.Count(p => p.CurrentStock <= 0),
                TotalStockValue = products.Sum(p => (decimal)p.CurrentStock * p.CostPrice),
                AverageCostPrice = products.Count > 0 ? products.Average(p => p.CostPrice) : 0
            };

            return ApiResponse<InventoryReportDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تقرير المخزون");
            return ApiResponse<InventoryReportDto>.Fail("حدث خطأ أثناء إنشاء تقرير المخزون");
        }
    }

    public async Task<ApiResponse<List<TopSellingProductDto>>> GetTopSellingAsync(ReportRequest request)
    {
        try
        {
            var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-1);
            var toDate = request.ToDate ?? DateTime.UtcNow;

            var saleItems = await _saleItemRepo.FindAsync(si =>
                !si.IsDeleted && si.Sale.Date >= fromDate && si.Sale.Date <= toDate);

            var topProducts = saleItems
                .GroupBy(si => new { si.ProductId, si.Product.Name, si.Product.SKU })
                .Select(g => new TopSellingProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    SKU = g.Key.SKU,
                    TotalQuantity = g.Sum(si => si.Quantity),
                    TotalRevenue = g.Sum(si => si.Total),
                    OrderCount = g.Select(si => si.SaleId).Distinct().Count()
                })
                .OrderByDescending(t => t.TotalQuantity)
                .Take(20)
                .ToList();

            return ApiResponse<List<TopSellingProductDto>>.Ok(topProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تقرير الأكثر مبيعاً");
            return ApiResponse<List<TopSellingProductDto>>.Fail("حدث خطأ أثناء إنشاء تقرير الأكثر مبيعاً");
        }
    }
}
