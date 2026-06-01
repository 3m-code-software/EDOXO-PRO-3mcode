using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Users;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<UserRole> _userRoleRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IGenericRepository<User> userRepo,
        IGenericRepository<UserRole> userRoleRepo,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllAsync(FilterRequest request)
    {
        try
        {
            var result = await _userRepo.GetPagedAsync(request);
            var items = _mapper.Map<List<UserDto>>(result.Items);

            foreach (var dto in items)
            {
                var user = result.Items.FirstOrDefault(u => u.Id == dto.Id);
                if (user?.Roles != null)
                    dto.Roles = user.Roles.Select(r => r.Role.Name).ToList();
            }

            var pagedResult = new PagedResult<UserDto>
            {
                Items = items,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return ApiResponse<PagedResult<UserDto>>.Ok(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قائمة المستخدمين");
            return ApiResponse<PagedResult<UserDto>>.Fail("حدث خطأ أثناء جلب المستخدمين");
        }
    }

    public async Task<ApiResponse<UserDto>> GetByIdAsync(int id)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<UserDto>.Fail("المستخدم غير موجود");

            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = user.Roles?.Select(r => r.Role.Name).ToList() ?? new List<string>();

            return ApiResponse<UserDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب المستخدم {UserId}", id);
            return ApiResponse<UserDto>.Fail("حدث خطأ أثناء جلب المستخدم");
        }
    }

    public async Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto request)
    {
        try
        {
            var existing = (await _userRepo.FindAsync(u =>
                u.Username == request.Username || u.Email == request.Email)).FirstOrDefault();

            if (existing != null)
                return ApiResponse<UserDto>.Fail("اسم المستخدم أو البريد الإلكتروني موجود بالفعل");

            var user = _mapper.Map<User>(request);
            user.PasswordHash = HashPassword(request.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _userRepo.AddAsync(user);

            if (request.RoleIds.Count != 0)
            {
                foreach (var roleId in request.RoleIds)
                {
                    await _userRoleRepo.AddAsync(new UserRole { UserId = user.Id, RoleId = roleId });
                }
            }

            var dto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.Ok(dto, "تم إنشاء المستخدم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء مستخدم جديد");
            return ApiResponse<UserDto>.Fail("حدث خطأ أثناء إنشاء المستخدم");
        }
    }

    public async Task<ApiResponse<UserDto>> UpdateAsync(int id, UpdateUserDto request)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<UserDto>.Fail("المستخدم غير موجود");

            if (request.Email != null) user.Email = request.Email;
            if (request.FullName != null) user.FullName = request.FullName;
            if (request.Phone != null) user.Phone = request.Phone;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;
            _userRepo.Update(user);

            if (request.RoleIds.Count != 0)
            {
                var existingRoles = (await _userRoleRepo.FindAsync(ur => ur.UserId == id)).ToList();
                foreach (var ur in existingRoles)
                    _userRoleRepo.Delete(ur);

                foreach (var roleId in request.RoleIds)
                    await _userRoleRepo.AddAsync(new UserRole { UserId = id, RoleId = roleId });
            }

            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = user.Roles?.Select(r => r.Role.Name).ToList() ?? new List<string>();

            return ApiResponse<UserDto>.Ok(dto, "تم تحديث المستخدم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث المستخدم {UserId}", id);
            return ApiResponse<UserDto>.Fail("حدث خطأ أثناء تحديث المستخدم");
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<string>.Fail("المستخدم غير موجود");

            _userRepo.SoftDelete(user);
            return ApiResponse<string>.Ok(string.Empty, "تم حذف المستخدم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف المستخدم {UserId}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء حذف المستخدم");
        }
    }

    public async Task<ApiResponse<string>> ActivateAsync(int id)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<string>.Fail("المستخدم غير موجود");

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepo.Update(user);

            var status = user.IsActive ? "تنشيط" : "تعطيل";
            return ApiResponse<string>.Ok(string.Empty, $"تم {status} المستخدم بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تغيير حالة المستخدم {UserId}", id);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تغيير حالة المستخدم");
        }
    }

    private static string HashPassword(string password)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
