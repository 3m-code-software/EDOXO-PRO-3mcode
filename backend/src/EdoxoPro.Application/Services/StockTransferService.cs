using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Inventory;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class StockTransferService : IStockTransferService
{
    private readonly IGenericRepository<StockTransfer> _transferRepo;
    private readonly IGenericRepository<StockTransferItem> _transferItemRepo;
    private readonly IGenericRepository<StockMovement> _movementRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<StockTransferService> _logger;

    public StockTransferService(
        IGenericRepository<StockTransfer> transferRepo,
        IGenericRepository<StockTransferItem> transferItemRepo,
        IGenericRepository<StockMovement> movementRepo,
        IGenericRepository<Product> productRepo,
        IMapper mapper,
        ILogger<StockTransferService> logger)
    {
        _transferRepo = transferRepo;
        _transferItemRepo = transferItemRepo;
        _movementRepo = movementRepo;
        _productRepo = productRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<StockTransferDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var items = await _transferRepo.FindAsync(t => !t.IsDeleted);
            var query = items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                query = query.Where(t => t.TransferNumber.ToLower().Contains(s));
            }

            var total = query.Count();
            var list = query.OrderByDescending(t => t.Date)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<StockTransferDto>>(list);
            var result = new PagedResult<StockTransferDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponse<PagedResult<StockTransferDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب تحويلات المخزون");
            return ApiResponse<PagedResult<StockTransferDto>>.Fail("حدث خطأ أثناء جلب تحويلات المخزون");
        }
    }

    public async Task<ApiResponse<StockTransferDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _transferRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<StockTransferDto>.Fail("تحويل المخزون غير موجود");

            var dto = _mapper.Map<StockTransferDto>(entity);
            return ApiResponse<StockTransferDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب تحويل المخزون {Id}", id);
            return ApiResponse<StockTransferDto>.Fail("حدث خطأ أثناء جلب تحويل المخزون");
        }
    }

    public async Task<ApiResponse<StockTransferDto>> CreateAsync(CreateStockTransferDto request)
    {
        try
        {
            var count = await _transferRepo.CountAsync();
            var transferNumber = $"TFR-{count + 1:D5}";

            var transfer = new StockTransfer
            {
                TransferNumber = transferNumber,
                FromWarehouseId = request.FromWarehouseId,
                ToWarehouseId = request.ToWarehouseId,
                Status = "Draft",
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _transferRepo.AddAsync(transfer);
            await _transferRepo.SaveChangesAsync();

            foreach (var itemReq in request.Items)
            {
                var item = new StockTransferItem
                {
                    TransferId = transfer.Id,
                    ProductId = itemReq.ProductId,
                    Quantity = itemReq.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                await _transferItemRepo.AddAsync(item);
            }

            await _transferItemRepo.SaveChangesAsync();

            var dto = _mapper.Map<StockTransferDto>(transfer);
            return ApiResponse<StockTransferDto>.Ok(dto, "تم إنشاء تحويل المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء تحويل مخزون");
            return ApiResponse<StockTransferDto>.Fail("حدث خطأ أثناء إنشاء تحويل المخزون");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _transferRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("تحويل المخزون غير موجود");

            if (entity.Status != "Draft")
                return ApiResponse<string>.Fail("لا يمكن حذف تحويل مؤكد");

            _transferRepo.SoftDelete(entity);
            await _transferRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم حذف تحويل المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف تحويل المخزون {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف تحويل المخزون");
        }
    }

    public async Task<ApiResponse<string>> ConfirmAsync(int id)
    {
        try
        {
            var entity = await _transferRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("تحويل المخزون غير موجود");

            if (entity.Status != "Draft")
                return ApiResponse<string>.Fail("تم تأكيد التحويل مسبقاً");

            var items = await _transferItemRepo.FindAsync(i => i.TransferId == id);

            foreach (var item in items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product == null) continue;

                product.CurrentStock -= item.Quantity;
                _productRepo.Update(product);

                var outMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = entity.FromWarehouseId,
                    Quantity = -item.Quantity,
                    Type = StockMovementType.Out,
                    ReferenceId = id,
                    ReferenceType = "StockTransfer",
                    Date = DateTime.UtcNow,
                    Notes = $"تحويل مخزون إلى المستودع {entity.ToWarehouseId}"
                };
                await _movementRepo.AddAsync(outMovement);

                var inMovement = new StockMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = entity.ToWarehouseId,
                    Quantity = item.Quantity,
                    Type = StockMovementType.In,
                    ReferenceId = id,
                    ReferenceType = "StockTransfer",
                    Date = DateTime.UtcNow,
                    Notes = $"تحويل مخزون من المستودع {entity.FromWarehouseId}"
                };
                await _movementRepo.AddAsync(inMovement);
            }

            entity.Status = "Confirmed";
            entity.UpdatedAt = DateTime.UtcNow;
            _transferRepo.Update(entity);
            await _transferRepo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم تأكيد تحويل المخزون بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تأكيد تحويل المخزون {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تأكيد تحويل المخزون");
        }
    }
}
