using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class DamagedStockService : IDamagedStockService
{
    private readonly IGenericRepository<DamagedStock> _damagedRepo;
    private readonly IGenericRepository<DamagedStockItem> _damagedItemRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<DamagedStockService> _logger;

    public DamagedStockService(
        IGenericRepository<DamagedStock> damagedRepo,
        IGenericRepository<DamagedStockItem> damagedItemRepo,
        IGenericRepository<Product> productRepo,
        IMapper mapper,
        ILogger<DamagedStockService> logger)
    {
        _damagedRepo = damagedRepo;
        _damagedItemRepo = damagedItemRepo;
        _productRepo = productRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<DamagedStockDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var items = await _damagedRepo.FindAsync(d => !d.IsDeleted);
            var query = items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(d => d.ReferenceNumber.ToLower().Contains(s));
            }

            var total = query.Count();
            var list = query.OrderByDescending(d => d.Date)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<DamagedStockDto>>(list);
            var result = new PagedResult<DamagedStockDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponse<PagedResult<DamagedStockDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المخزون التالف");
            return ApiResponse<PagedResult<DamagedStockDto>>.Fail("حدث خطأ أثناء جلب المخزون التالف");
        }
    }

    public async Task<ApiResponse<DamagedStockDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _damagedRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<DamagedStockDto>.Fail("سجل التالف غير موجود");

            var dto = _mapper.Map<DamagedStockDto>(entity);
            return ApiResponse<DamagedStockDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب سجل التالف {Id}", id);
            return ApiResponse<DamagedStockDto>.Fail("حدث خطأ أثناء جلب سجل التالف");
        }
    }

    public async Task<ApiResponse<DamagedStockDto>> CreateAsync(CreateDamagedStockDto request)
    {
        try
        {
            var count = await _damagedRepo.CountAsync();
            var refNumber = $"DMG-{count + 1:D5}";

            var damaged = new DamagedStock
            {
                ReferenceNumber = refNumber,
                WarehouseId = request.WarehouseId,
                Date = DateTime.UtcNow,
                Reason = request.Reason,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _damagedRepo.AddAsync(damaged);
            await _damagedRepo.SaveChangesAsync();

            decimal total = 0;

            foreach (var itemReq in request.Items)
            {
                var itemTotal = (decimal)(itemReq.Quantity * (double)itemReq.UnitPrice);
                var item = new DamagedStockItem
                {
                    DamagedStockId = damaged.Id,
                    ProductId = itemReq.ProductId,
                    Quantity = itemReq.Quantity,
                    UnitPrice = itemReq.UnitPrice,
                    Total = (decimal)itemTotal,
                    CreatedAt = DateTime.UtcNow
                };

                total += (decimal)itemTotal;
                await _damagedItemRepo.AddAsync(item);

                var product = await _productRepo.GetByIdAsync(itemReq.ProductId);
                if (product != null)
                {
                    product.CurrentStock -= itemReq.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    _productRepo.Update(product);
                }
            }

            damaged.Total = (decimal)total;
            _damagedRepo.Update(damaged);
            await _damagedRepo.SaveChangesAsync();

            var dto = _mapper.Map<DamagedStockDto>(damaged);
            return ApiResponse<DamagedStockDto>.Ok(dto, "تم إنشاء سجل التالف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء سجل تالف");
            return ApiResponse<DamagedStockDto>.Fail("حدث خطأ أثناء إنشاء سجل التالف");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _damagedRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("سجل التالف غير موجود");

            _damagedRepo.SoftDelete(entity);
            await _damagedRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم حذف سجل التالف بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف سجل التالف {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف سجل التالف");
        }
    }
}
