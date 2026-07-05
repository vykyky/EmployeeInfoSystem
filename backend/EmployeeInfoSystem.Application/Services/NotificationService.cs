using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Notification;
using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Application.Interfaces.Services;
using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;  

        public NotificationService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ── Внутренние методы (вызываются из RequestService) ─────────────────

        public async Task NotifyNewRequestAsync(Request request)
        {
            // Если менеджер уже назначен — уведомляем его.
            // Если нет — уведомляем всех менеджеров и админов (они увидят заявку в общем списке).
            List<int> recipientIds;

            if (request.ManagerId.HasValue)
            {
                recipientIds = new List<int> { request.ManagerId.Value };
            }
            else
            {
                var managers = await _uow.Users.GetByRolesAsync(new[] { "manager", "admin" });
                recipientIds = managers.Select(u => u.Id).ToList();
            }

            var notifications = recipientIds.Select(rid => new Notification
            {
                RecipientId = rid,
                SenderId = request.EmployeeId,
                Title = "Новый запрос",
                Body = $"Поступил новый запрос. Тип: {request.RequestType?.Name ?? request.RequestTypeId.ToString()}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RequestId = request.Id
            });

            await _uow.Notifications.AddRangeAsync(notifications);
        }

        public async Task NotifyRequestResolvedAsync(Request request)
        {
            // Уведомляем сотрудника о том что запрос завершён
            await _uow.Notifications.AddAsync(new Notification
            {
                RecipientId = request.EmployeeId,
                SenderId = request.ManagerId,
                Title = "Запрос выполнен",
                Body = request.ResolutionComment ?? "Ваш запрос был выполнен.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RequestId = request.Id
            });
        }

        public async Task NotifyManagerAssignedAsync(Request request)
        {
            if (!request.ManagerId.HasValue) return;

            await _uow.Notifications.AddAsync(new Notification
            {
                RecipientId = request.ManagerId.Value,
                SenderId = null,   // системное
                Title = "Вам назначен запрос",
                Body = $"Запрос #{request.Id} назначен вам.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RequestId = request.Id
            });
        }

        // ── Самостоятельная рассылка (Рис. 5-7) ──────────────────────────────

        public async Task<Result> SendAsync(int senderId, SendNotificationDto dto)
        {
            var recipientIds = new HashSet<int>();

            if (dto.RecipientGroupId.HasValue)
            {
                var group = await _uow.RecipientGroups.GetByIdAsync(dto.RecipientGroupId.Value);
                if (group is null)
                    return Error.NotFound($"Группа получателей {dto.RecipientGroupId} не найдена");

                var users = await _uow.Users.GetByFilterAsync(group.Department, group.Role);
                foreach (var id in users.Select(u => u.Id)) recipientIds.Add(id);
            }

            if (dto.RecipientUserIds is { Count: > 0 })
                foreach (var id in dto.RecipientUserIds) recipientIds.Add(id);

            if (recipientIds.Count == 0)
                return Error.Validation("Не указаны получатели уведомления");

            var notifications = recipientIds.Select(rid => new Notification
            {
                RecipientId = rid,
                SenderId = senderId,
                Title = dto.Title,
                Body = dto.Body,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RequestId = null   // не задача, просто уведомление
            });

            await _uow.Notifications.AddRangeAsync(notifications);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        // ── Чтение ────────────────────────────────────────────────────────────

        public async Task<List<NotificationDto>> GetMyNotificationsAsync(int userId)
        {
            var list = await _uow.Notifications.GetMyNotificationsAsync(userId);
            return list.Select(ToDto).ToList();
        }

        public async Task<List<NotificationDto>> GetMyTasksAsync(int userId)
        {
            var list = await _uow.Notifications.GetTasksAsync(userId);
            return list.Select(ToDto).ToList();
        }

        public async Task<List<NotificationDto>> GetAllTasksAsync()
        {
            var list = await _uow.Notifications.GetAllTasksAsync();
            return list.Select(ToDto).ToList();
        }

        public async Task<Result> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _uow.Notifications.GetByIdAsync(notificationId);
            if (notification is null)
                return Error.NotFound($"Уведомление {notificationId} не найдено");

            if (notification.RecipientId != userId)
                return Error.Forbidden("Нет доступа к этому уведомлению");

            notification.IsRead = true;
            await _uow.Notifications.UpdateAsync(notification);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        private static NotificationDto ToDto(Notification n) => new()
        {
            Id = n.Id,
            RecipientId = n.RecipientId,
            SenderId = n.SenderId,
            SenderFio = n.Sender?.EmployeeProfile?.Fio,
            Title = n.Title,
            Body = n.Body,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            RequestId = n.RequestId,
            RequestStatus = n.Request?.Status
        };
    }
}
