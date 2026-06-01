using AutoMapper;
using ClosedXML.Excel;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IGenericRepository<Supplier> _supplierRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(
        IGenericRepository<Supplier> supplierRepo,
        IMapper mapper,
        ILogger<SupplierService> logger)
    {
        _supplierRepo = supplierRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<SupplierDto>>> GetAllAsync(SupplierFilterRequest request)
    {
        try
        {
            var commonRequest = new Common.FilterRequest
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search,
                SortBy = request.SortBy,
                SortDesc = request.SortDirection == "desc"
            };
            var result = await _supplierRepo.GetPagedAsync(commonRequest);
            var items = _mapper.Map<List<SupplierDto>>(result.Items);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                items = items.Where(i =>
                    i.Name.ToLower().Contains(s) ||
                    (i.TaxNumber != null && i.TaxNumber.ToLower().Contains(s))).ToList();
            }

            if (request.PaymentPeriod.HasValue)
                items = items.Where(i => i.PaymentPeriod == request.PaymentPeriod.Value).ToList();

            if (request.IsActive.HasValue)
                items = items.Where(i => i.IsActive == request.IsActive.Value).ToList();

            var pagedResult = new PagedResult<SupplierDto>
            {
                Items = items,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return ApiResponse<PagedResult<SupplierDto>>.Ok(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة الموردين");
            return ApiResponse<PagedResult<SupplierDto>>.Fail("حدث خطأ أثناء جلب الموردين");
        }
    }

    public async Task<ApiResponse<SupplierDto>> GetByIdAsync(int id)
    {
        try
        {
            var supplier = await _supplierRepo.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<SupplierDto>.Fail("المورد غير موجود");

            var dto = _mapper.Map<SupplierDto>(supplier);
            return ApiResponse<SupplierDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المورد {SupplierId}", id);
            return ApiResponse<SupplierDto>.Fail("حدث خطأ أثناء جلب المورد");
        }
    }

    public async Task<ApiResponse<SupplierDto>> CreateAsync(CreateSupplierDto request)
    {
        try
        {
            var supplier = _mapper.Map<Supplier>(request);
            supplier.CreatedAt = DateTime.UtcNow;
            supplier.IsActive = true;

            await _supplierRepo.AddAsync(supplier);

            var dto = _mapper.Map<SupplierDto>(supplier);
            return ApiResponse<SupplierDto>.Ok(dto, "تم إنشاء المورد بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء مورد جديد");
            return ApiResponse<SupplierDto>.Fail("حدث خطأ أثناء إنشاء المورد");
        }
    }

    public async Task<ApiResponse<SupplierDto>> UpdateAsync(int id, UpdateSupplierDto request)
    {
        try
        {
            var supplier = await _supplierRepo.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<SupplierDto>.Fail("المورد غير موجود");

            _mapper.Map(request, supplier);
            supplier.UpdatedAt = DateTime.UtcNow;
            _supplierRepo.Update(supplier);

            var dto = _mapper.Map<SupplierDto>(supplier);
            return ApiResponse<SupplierDto>.Ok(dto, "تم تحديث المورد بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث المورد {SupplierId}", id);
            return ApiResponse<SupplierDto>.Fail("حدث خطأ أثناء تحديث المورد");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var supplier = await _supplierRepo.GetByIdAsync(id);
            if (supplier == null)
                return ApiResponse<string>.Fail("المورد غير موجود");

            _supplierRepo.SoftDelete(supplier);
            return ApiResponse<string>.Ok(string.Empty, "تم حذف المورد بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف المورد {SupplierId}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف المورد");
        }
    }

    public async Task<ApiResponse<byte[]>> ExportAsync(FilterRequest request)
    {
        try
        {
            var suppliers = (await _supplierRepo.GetAllAsync()).ToList();
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.ToLower();
                suppliers = suppliers.Where(i =>
                    i.Name.ToLower().Contains(s) ||
                    (i.TaxNumber != null && i.TaxNumber.ToLower().Contains(s))).ToList();
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("الموردين");

            ws.Cell(1, 1).Value = "الاسم";
            ws.Cell(1, 2).Value = "رقم الاتصال";
            ws.Cell(1, 3).Value = "البريد الإلكتروني";
            ws.Cell(1, 4).Value = "الهاتف";
            ws.Cell(1, 5).Value = "الرقم الضريبي";
            ws.Cell(1, 6).Value = "الرصيد الافتتاحي";
            ws.Cell(1, 7).Value = "الحد الائتماني";
            ws.Cell(1, 8).Value = "المدينة";
            ws.Cell(1, 9).Value = "نشط";

            var headerRange = ws.Range(1, 1, 1, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            var row = 2;
            foreach (var s in suppliers)
            {
                ws.Cell(row, 1).Value = s.Name;
                ws.Cell(row, 2).Value = s.ContactId;
                ws.Cell(row, 3).Value = s.Email ?? "";
                ws.Cell(row, 4).Value = s.Phone ?? "";
                ws.Cell(row, 5).Value = s.TaxNumber ?? "";
                ws.Cell(row, 6).Value = s.OpeningBalance;
                ws.Cell(row, 7).Value = s.CreditLimit ?? 0;
                ws.Cell(row, 8).Value = s.City ?? "";
                ws.Cell(row, 9).Value = s.IsActive ? "نعم" : "لا";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return ApiResponse<byte[]>.Ok(content, "تم تصدير الموردين بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تصدير الموردين");
            return ApiResponse<byte[]>.Fail("حدث خطأ أثناء تصدير الموردين");
        }
    }
}
