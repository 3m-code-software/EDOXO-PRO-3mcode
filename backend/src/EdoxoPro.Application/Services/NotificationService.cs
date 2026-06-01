using AutoMapper;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Shared;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EdoxoPro.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IGenericRepository<Notification> _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IGenericRepository<Notification> repo,
        IMapper mapper,
        ILogger<NotificationService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<List<NotificationDto>>> GetUserNotificationsAsync(int userId)
    {
        try
        {
            var notifications = await _repo.FindAsync(n => n.UserId == userId && !n.IsDeleted);
            var list = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt
                })
                .ToList();

            return ApiResponse<List<NotificationDto>>.Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب إشعارات المستخدم {UserId}", userId);
            return ApiResponse<List<NotificationDto>>.Fail("حدث خطأ أثناء جلب الإشعارات");
        }
    }

    public async Task<ApiResponse<string>> MarkAsReadAsync(int notificationId)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(notificationId);
            if (entity == null || entity.IsDeleted)
                return ApiResponse<string>.Fail("الإشعار غير موجود");

            entity.IsRead = true;
            entity.ReadAt = DateTime.UtcNow;
            _repo.Update(entity);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم تحديث الإشعار كمقروء");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث الإشعار {NotificationId}", notificationId);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تحديث الإشعار");
        }
    }

    public async Task<ApiResponse<string>> MarkAllAsReadAsync(int userId)
    {
        try
        {
            var notifications = await _repo.FindAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
                _repo.Update(n);
            }

            await _repo.SaveChangesAsync();

            return ApiResponse<string>.Ok(string.Empty, "تم تحديث جميع الإشعارات كمقروءة");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث جميع إشعارات المستخدم {UserId}", userId);
            return ApiResponse<string>.Fail("حدث خطأ أثناء تحديث الإشعارات");
        }
    }

    public async Task<ApiResponse<NotificationDto>> CreateAsync(CreateNotificationDto request)
    {
        try
        {
            var entity = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            var dto = new NotificationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title,
                Message = entity.Message,
                Type = entity.Type,
                IsRead = entity.IsRead,
                ReadAt = entity.ReadAt,
                CreatedAt = entity.CreatedAt
            };

            return ApiResponse<NotificationDto>.Ok(dto, "تم إنشاء الإشعار بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء إشعار جديد");
            return ApiResponse<NotificationDto>.Fail("حدث خطأ أثناء إنشاء الإشعار");
        }
    }
}
