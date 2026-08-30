using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Notification;
using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface INotificationService
    {
        // Самостоятельная рассылка (Рис. 5-7), RequestId не указывается
        Task<Result> SendAsync(int senderId, SendNotificationDto dto);

        // Внутренние методы -- вызываются из RequestService при смене статуса заявки
        Task NotifyNewRequestAsync(Request request);
        Task NotifyRequestAssignedAsync(Request request);
        Task NotifyRequestInProgressAsync(Request request);
        Task NotifyRequestResolvedAsync(Request request);

        Task<List<NotificationDto>> GetMyNotificationsAsync(int userId);
        Task<List<NotificationDto>> GetMyTasksAsync(int userId);
        Task<List<NotificationDto>> GetAllTasksAsync();

        Task<Result> DeleteAsync(int notificationId, int userId);
        Task<Result> MarkAsReadAsync(int notificationId, int userId);
    }
}
