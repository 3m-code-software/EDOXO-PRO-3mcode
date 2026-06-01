using EdoxoPro.Application.Common;
using EdoxoPro.Application.DTOs.Shared;

namespace EdoxoPro.Application.Interfaces;
public interface INotificationService
{
    Task<ApiResponse<List<NotificationDto>>> GetUserNotificationsAsync(int userId);
    Task<ApiResponse<string>> MarkAsReadAsync(int notificationId);
    Task<ApiResponse<string>> MarkAllAsReadAsync(int userId);
    Task<ApiResponse<NotificationDto>> CreateAsync(CreateNotificationDto request);
}
