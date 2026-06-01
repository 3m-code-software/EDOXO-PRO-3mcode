using AutoMapper;
using ClosedXML.Excel;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Sales;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class SaleService : ISaleService
{
    private readonly IGenericRepository<Sale> _saleRepo;
    private readonly IGenericRepository<SaleItem> _saleItemRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<StockMovement> _stockMovementRepo;
    private readonly IGenericRepository<InvoiceSetting> _invoiceSettingRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<SaleService> _logger;

    public SaleService(
        IGenericRepository<Sale> saleRepo,
        IGenericRepository<SaleItem> saleItemRepo,
        IGenericRepository<Product> productRepo,
        IGenericRepository<StockMovement> stockMovementRepo,
        IGenericRepository<InvoiceSetting> invoiceSettingRepo,
        IMapper mapper,
        ILogger<SaleService> logger)
    {
        _saleRepo = saleRepo;
        _saleItemRepo = saleItemRepo;
        _productRepo = productRepo;
        _stockMovementRepo = stockMovementRepo;
        _invoiceSettingRepo = invoiceSettingRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<SaleDto>>> GetAllAsync(SaleFilterRequest request)
    {
        try
        {
            var items = await _saleRepo.FindAsync(s => !s.IsDeleted);
            var query = items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status.ToString() == request.Status);
            if (request.CustomerId.HasValue)
                query = query.Where(s => s.CustomerId == request.CustomerId.Value);
            if (request.DateFrom.HasValue)
                query = query.Where(s => s.Date >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(s => s.Date <= request.DateTo.Value);
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(s => s.InvoiceNumber.ToLower().Contains(request.Search.ToLower()));

            var total = query.Count();
            var list = query.OrderByDescending(s => s.Date).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            var dtos = _mapper.Map<List<SaleDto>>(list);
            var result = new PagedResult<SaleDto> { Items = dtos, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
            return ApiResponse<PagedResult<SaleDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة الفواتير");
            return ApiResponse<PagedResult<SaleDto>>.Fail("حدث خطأ أثناء جلب الفواتير");
        }
    }

    public async Task<ApiResponse<SaleDto>> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _saleRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<SaleDto>.Fail("الفاتورة غير موجودة");
            var dto = _mapper.Map<SaleDto>(entity);
            return ApiResponse<SaleDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الفاتورة {Id}", id);
            return ApiResponse<SaleDto>.Fail("حدث خطأ أثناء جلب الفاتورة");
        }
    }

    public async Task<ApiResponse<SaleDto>> CreateAsync(CreateSaleDto request)
    {
        try
        {
            var invoiceSetting = (await _invoiceSettingRepo.GetAllAsync()).FirstOrDefault();
            var nextNumber = invoiceSetting?.NextNumber ?? 1;
            var prefix = invoiceSetting?.Prefix ?? "INV-";
            var invoiceNumber = $"{prefix}{nextNumber:D5}";
            if (invoiceSetting != null)
            {
                invoiceSetting.NextNumber++;
                _invoiceSettingRepo.Update(invoiceSetting);
            }

            var sale = new Sale
            {
                InvoiceNumber = invoiceNumber,
                CustomerId = request.CustomerId,
                DelegateId = request.DelegateId,
                BranchId = request.BranchId,
                Date = request.Date ?? DateTime.UtcNow,
                Discount = request.Discount,
                DiscountType = request.DiscountType,
                TaxRate = request.TaxRate,
                Notes = request.Notes,
                Status = SaleStatus.Draft,
                PaymentStatus = PaymentStatus.Unpaid,
                ShippingStatus = ShippingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            decimal subtotal = 0;
            var items = new List<SaleItem>();

            foreach (var itemReq in request.Items)
            {
                var product = await _productRepo.GetByIdAsync(itemReq.ProductId);
                if (product == null || product.IsDeleted)
                    return ApiResponse<SaleDto>.Fail($"المنتج {itemReq.ProductId} غير موجود");
                if (product.CurrentStock < itemReq.Quantity)
                    return ApiResponse<SaleDto>.Fail($"المنتج {product.Name} لا توجد كمية كافية في المخزون");

                var itemTotal = (decimal)(itemReq.Quantity * (double)itemReq.UnitPrice);
                if (itemReq.DiscountType == "Percent")
                    itemTotal -= itemTotal * itemReq.Discount / 100;
                else
                    itemTotal -= itemReq.Discount;

                items.Add(new SaleItem
                {
                    ProductId = itemReq.ProductId,
                    VariantId = itemReq.VariantId,
                    Quantity = itemReq.Quantity,
                    UnitPrice = itemReq.UnitPrice,
                    Discount = itemReq.Discount,
                    DiscountType = itemReq.DiscountType,
                    Total = itemTotal,
                    CreatedAt = DateTime.UtcNow
                });

                subtotal += itemTotal;

                product.CurrentStock -= itemReq.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
                _productRepo.Update(product);

                await _stockMovementRepo.AddAsync(new StockMovement
                {
                    ProductId = product.Id,
                    WarehouseId = 1,
                    Quantity = -itemReq.Quantity,
                    Type = StockMovementType.Out,
                    Date = DateTime.UtcNow,
                    ReferenceType = "Sale",
                    Notes = $"فاتورة مبيعات {invoiceNumber}"
                });
            }

            sale.Subtotal = subtotal;
            if (sale.DiscountType == "Percent")
                sale.Discount = subtotal * sale.Discount / 100;
            sale.Tax = (subtotal - sale.Discount) * sale.TaxRate / 100;
            sale.Total = subtotal - sale.Discount + sale.Tax;

            await _saleRepo.AddAsync(sale);
            await _saleRepo.SaveChangesAsync();

            foreach (var item in items)
            {
                item.SaleId = sale.Id;
                await _saleItemRepo.AddAsync(item);
            }
            await _saleItemRepo.SaveChangesAsync();

            var dto = _mapper.Map<SaleDto>(sale);
            return ApiResponse<SaleDto>.Ok(dto, "تم إنشاء الفاتورة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء فاتورة جديدة");
            return ApiResponse<SaleDto>.Fail("حدث خطأ أثناء إنشاء الفاتورة");
        }
    }

    public async Task<ApiResponse<SaleDto>> UpdateAsync(int id, UpdateSaleDto request)
    {
        try
        {
            var entity = await _saleRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<SaleDto>.Fail("الفاتورة غير موجودة");
            if (!string.IsNullOrWhiteSpace(request.Status))
                entity.Status = Enum.Parse<SaleStatus>(request.Status);
            if (request.Notes != null)
                entity.Notes = request.Notes;
            entity.UpdatedAt = DateTime.UtcNow;
            _saleRepo.Update(entity);
            await _saleRepo.SaveChangesAsync();
            var dto = _mapper.Map<SaleDto>(entity);
            return ApiResponse<SaleDto>.Ok(dto, "تم تحديث الفاتورة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الفاتورة {Id}", id);
            return ApiResponse<SaleDto>.Fail("حدث خطأ أثناء تحديث الفاتورة");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var entity = await _saleRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("الفاتورة غير موجودة");
            if (entity.Status == SaleStatus.Confirmed)
                return ApiResponse<string>.Fail("لا يمكن حذف فاتورة مؤكدة");
            _saleRepo.SoftDelete(entity);
            await _saleRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم حذف الفاتورة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف الفاتورة {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف الفاتورة");
        }
    }

    public async Task<ApiResponse<string>> ApproveAsync(int id)
    {
        try
        {
            var entity = await _saleRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("الفاتورة غير موجودة");
            if (entity.Status != SaleStatus.Draft)
                return ApiResponse<string>.Fail("الفاتورة ليست في حالة مسودة");
            entity.Status = SaleStatus.Confirmed;
            entity.UpdatedAt = DateTime.UtcNow;
            _saleRepo.Update(entity);
            await _saleRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم اعتماد الفاتورة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في اعتماد الفاتورة {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء اعتماد الفاتورة");
        }
    }

    public async Task<ApiResponse<string>> PayAsync(int id, PaySaleDto request)
    {
        try
        {
            var entity = await _saleRepo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("الفاتورة غير موجودة");
            entity.PaidAmount += request.Amount;
            entity.PaymentStatus = entity.PaidAmount >= entity.Total ? PaymentStatus.Paid : PaymentStatus.Partial;
            entity.UpdatedAt = DateTime.UtcNow;
            _saleRepo.Update(entity);
            await _saleRepo.SaveChangesAsync();
            return ApiResponse<string>.Ok(string.Empty, "تم تسجيل الدفعة بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تسجيل دفعة للفاتورة {Id}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تسجيل الدفعة");
        }
    }

    public async Task<ApiResponse<IEnumerable<SaleDto>>> GetDraftsAsync()
    {
        try
        {
            var entities = await _saleRepo.FindAsync(s => s.Status == SaleStatus.Draft && !s.IsDeleted);
            var dtos = _mapper.Map<List<SaleDto>>(entities);
            return ApiResponse<IEnumerable<SaleDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المسودات");
            return ApiResponse<IEnumerable<SaleDto>>.Fail("حدث خطأ أثناء جلب المسودات");
        }
    }

    public async Task<ApiResponse<IEnumerable<SaleDto>>> GetQuotesAsync()
    {
        try
        {
            var entities = await _saleRepo.FindAsync(s => s.Status == SaleStatus.Draft && !s.IsDeleted);
            var dtos = _mapper.Map<List<SaleDto>>(entities);
            return ApiResponse<IEnumerable<SaleDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب عروض الأسعار");
            return ApiResponse<IEnumerable<SaleDto>>.Fail("حدث خطأ أثناء جلب عروض الأسعار");
        }
    }

    public async Task<ApiResponse<byte[]>> ExportAsync(SaleFilterRequest request)
    {
        try
        {
            var items = await _saleRepo.FindAsync(s => !s.IsDeleted);
            var query = items.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status.ToString() == request.Status);
            if (request.DateFrom.HasValue)
                query = query.Where(s => s.Date >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(s => s.Date <= request.DateTo.Value);

            var list = query.OrderByDescending(s => s.Date).ToList();
            var dtos = _mapper.Map<List<SaleDto>>(list);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("المبيعات");
            ws.Cell(1, 1).Value = "رقم الفاتورة";
            ws.Cell(1, 2).Value = "التاريخ";
            ws.Cell(1, 3).Value = "الإجمالي";
            ws.Cell(1, 4).Value = "المدفوع";
            ws.Cell(1, 5).Value = "الحالة";
            ws.Cell(1, 6).Value = "حالة الدفع";
            var headerRange = ws.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            var row = 2;
            foreach (var dto in dtos)
            {
                ws.Cell(row, 1).Value = dto.InvoiceNumber;
                ws.Cell(row, 2).Value = dto.Date.ToString("yyyy-MM-dd");
                ws.Cell(row, 3).Value = dto.Total;
                ws.Cell(row, 4).Value = dto.PaidAmount;
                ws.Cell(row, 5).Value = dto.Status;
                ws.Cell(row, 6).Value = dto.PaymentStatus;
                row++;
            }
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return ApiResponse<byte[]>.Ok(content, "تم تصدير المبيعات بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تصدير المبيعات");
            return ApiResponse<byte[]>.Fail("حدث خطأ أثناء تصدير المبيعات");
        }
    }
}
