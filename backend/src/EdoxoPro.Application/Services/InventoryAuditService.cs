using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class InventoryAuditService : IInventoryAuditService
{
    private readonly IGenericRepository<InventoryAudit> _auditRepo;
    private readonly IGenericRepository<InventoryAuditItem> _auditItemRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<InventoryAuditService> _logger;

    public InventoryAuditService(
        IGenericRepository<InventoryAudit> auditRepo,
        IGenericRepository<InventoryAuditItem> auditItemRepo,
        IGenericRepository<Product> productRepo,
        IMapper mapper,
        ILogger<InventoryAuditService> logger)
    {
        _auditRepo = auditRepo;
        _auditItemRepo = auditItemRepo;
        _productRepo = productRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<InventoryAuditDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var items = await _auditRepo.FindAsync(a => !a.IsDeleted);
            var query = items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(a => a.AuditNumber.ToLower().Contains(s));
            }

            var total = query.Count();
            var list = query.OrderByDescending(a => a.Date)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<InventoryAuditDto>>(list);
            var result = new PagedResult<InventoryAuditDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponse<PagedResult<InventoryAuditDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب جرد المخزون");
            return ApiResponse<PagedResult<InventoryAuditDto>>.Fail("حدث خطأ أثناء جلب جرد المخزون");
        }
    }

    public async Task<ApiResponse<InventoryAuditDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _auditRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<InventoryAuditDto>.Fail("جرد المخزون غير موجود");

            var dto = _mapper.Map<InventoryAuditDto>(entity);
            return ApiResponse<InventoryAuditDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب جرد المخزون {Id}", id);
            return ApiResponse<InventoryAuditDto>.Fail("حدث خطأ أثناء جلب جرد المخزون");
        }
    }

    public async Task<ApiResponse<InventoryAuditDto>> CreateAsync(CreateInventoryAuditDto request)
    {
        try
        {
            var count = await _auditRepo.CountAsync();
            var auditNumber = $"AUD-{count + 1:D5}";

            var audit = new InventoryAudit
            {
                AuditNumber = auditNumber,
                WarehouseId = request.WarehouseId,
                Date = DateTime.UtcNow,
                Status = InventoryAuditStatus.Draft,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _auditRepo.AddAsync(audit);
            await _auditRepo.SaveChangesAsync();

            foreach (var itemReq in request.Items)
            {
                var difference = itemReq.ActualQuantity - itemReq.SystemQuantity;
                var auditItem = new InventoryAuditItem
                {
                    AuditId = audit.Id,
                    ProductId = itemReq.ProductId,
                    SystemQuantity = itemReq.SystemQuantity,
                    ActualQuantity = itemReq.ActualQuantity,
                    Difference = difference,
                    UnitPrice = itemReq.UnitPrice,
                    CreatedAt = DateTime.UtcNow
                };
                await _auditItemRepo.AddAsync(auditItem);
            }

            await _auditItemRepo.SaveChangesAsync();

            var dto = _mapper.Map<InventoryAuditDto>(audit);
            return ApiResponse<InventoryAuditDto>.Ok(dto, "تم إنشاء جرد المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء جرد مخزون");
            return ApiResponse<InventoryAuditDto>.Fail("حدث خطأ أثناء إنشاء جرد المخزون");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _auditRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("جرد المخزون غير موجود");

            if (entity.Status == InventoryAuditStatus.Completed)
                return ApiResponse<string>.Fail("لا يمكن حذف جرد مكتمل");

            _auditRepo.SoftDelete(entity);
            await _auditRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم حذف جرد المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف جرد المخزون {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف جرد المخزون");
        }
    }

    public async Task<ApiResponse<string>> StartAsync(int id)
    {
        try
        {
            var entity = await _auditRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("جرد المخزون غير موجود");

            if (entity.Status != InventoryAuditStatus.Draft)
                return ApiResponse<string>.Fail("يمكن بدء الجرد من الحالة مسودة فقط");

            entity.Status = InventoryAuditStatus.InProgress;
            entity.UpdatedAt = DateTime.UtcNow;
            _auditRepo.Update(entity);
            await _auditRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم بدء جرد المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في بدء جرد المخزون {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء بدء جرد المخزون");
        }
    }

    public async Task<ApiResponse<string>> CompleteAsync(int id)
    {
        try
        {
            var entity = await _auditRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("جرد المخزون غير موجود");

            if (entity.Status != InventoryAuditStatus.InProgress)
                return ApiResponse<string>.Fail("يمكن إكمال الجرد من الحالة قيد التنفيذ فقط");

            var auditItems = await _auditItemRepo.FindAsync(i => i.AuditId == id);

            foreach (var auditItem in auditItems)
            {
                var product = await _productRepo.GetByIdAsync(auditItem.ProductId);
                if (product != null)
                {
                    product.CurrentStock = auditItem.ActualQuantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    _productRepo.Update(product);
                }
            }

            entity.Status = InventoryAuditStatus.Completed;
            entity.UpdatedAt = DateTime.UtcNow;
            _auditRepo.Update(entity);
            await _auditRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم إكمال جرد المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إكمال جرد المخزون {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء إكمال جرد المخزون");
        }
    }
}
