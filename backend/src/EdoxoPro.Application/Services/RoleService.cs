using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Roles;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class RoleService : IRoleService
{
    private readonly IGenericRepository<Role> _roleRepo;
    private readonly IGenericRepository<RolePermission> _permissionRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IGenericRepository<Role> roleRepo,
        IGenericRepository<RolePermission> permissionRepo,
        IMapper mapper,
        ILogger<RoleService> logger)
    {
        _roleRepo = roleRepo;
        _permissionRepo = permissionRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<RoleDto>>> GetAllAsync()
    {
        try
        {
            var roles = await _roleRepo.GetAllAsync();
            var dtos = _mapper.Map<List<RoleDto>>(roles);

            foreach (var dto in dtos)
            {
                var role = roles.FirstOrDefault(r => r.Id == dto.Id);
                if (role?.Users != null)
                    dto.PermissionCount = role.Users.Count;
            }

            return ApiResponse<IEnumerable<RoleDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة الأدوار");
            return ApiResponse<IEnumerable<RoleDto>>.Fail("حدث خطأ أثناء جلب الأدوار");
        }
    }

    public async Task<ApiResponse<RoleDto>> GetByIdAsync(int id)
    {
        try
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role == null)
                return ApiResponse<RoleDto>.Fail("الدور غير موجود");

            var dto = _mapper.Map<RoleDto>(role);
            var permissions = (await _permissionRepo.FindAsync(p => p.RoleId == id)).ToList();
            dto.Permissions = permissions.Select(p => p.PermissionName).ToList();
            dto.PermissionCount = permissions.Count;

            return ApiResponse<RoleDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الدور {RoleId}", id);
            return ApiResponse<RoleDto>.Fail("حدث خطأ أثناء جلب الدور");
        }
    }

    public async Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto request)
    {
        try
        {
            var existing = (await _roleRepo.FindAsync(r => r.Name == request.Name)).FirstOrDefault();
            if (existing != null)
                return ApiResponse<RoleDto>.Fail("اسم الدور موجود بالفعل");

            var role = _mapper.Map<Role>(request);
            await _roleRepo.AddAsync(role);

            if (request.Permissions.Count != 0)
            {
                foreach (var perm in request.Permissions)
                {
                    await _permissionRepo.AddAsync(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionName = perm
                    });
                }
            }

            var dto = _mapper.Map<RoleDto>(role);
            dto.Permissions = request.Permissions;
            dto.PermissionCount = request.Permissions.Count;

            return ApiResponse<RoleDto>.Ok(dto, "تم إنشاء الدور بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء دور جديد");
            return ApiResponse<RoleDto>.Fail("حدث خطأ أثناء إنشاء الدور");
        }
    }

    public async Task<ApiResponse<RoleDto>> UpdateAsync(int id, UpdateRoleDto request)
    {
        try
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role == null)
                return ApiResponse<RoleDto>.Fail("الدور غير موجود");

            if (request.Name != null) role.Name = request.Name;
            if (request.NameAr != null) role.NameAr = request.NameAr;
            if (request.Description != null) role.Description = request.Description;
            _roleRepo.Update(role);

            if (request.Permissions != null)
            {
                var existingPerms = (await _permissionRepo.FindAsync(p => p.RoleId == id)).ToList();
                foreach (var p in existingPerms)
                    _permissionRepo.Delete(p);

                foreach (var perm in request.Permissions)
                {
                    await _permissionRepo.AddAsync(new RolePermission
                    {
                        RoleId = id,
                        PermissionName = perm
                    });
                }
            }

            var dto = _mapper.Map<RoleDto>(role);
            var permissions = request.Permissions ??
                (await _permissionRepo.FindAsync(p => p.RoleId == id)).Select(p => p.PermissionName).ToList();
            dto.Permissions = permissions;
            dto.PermissionCount = permissions.Count;

            return ApiResponse<RoleDto>.Ok(dto, "تم تحديث الدور بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الدور {RoleId}", id);
            return ApiResponse<RoleDto>.Fail("حدث خطأ أثناء تحديث الدور");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var role = await _roleRepo.GetByIdAsync(id);
            if (role == null)
                return ApiResponse<string>.Fail("الدور غير موجود");

            if (role.IsSystem)
                return ApiResponse<string>.Fail("لا يمكن حذف أدوار النظام");

            var permissions = (await _permissionRepo.FindAsync(p => p.RoleId == id)).ToList();
            foreach (var p in permissions)
                _permissionRepo.Delete(p);

            _roleRepo.Delete(role);
            return ApiResponse<string>.Ok(string.Empty, "تم حذف الدور بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف الدور {RoleId}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف الدور");
        }
    }
}
