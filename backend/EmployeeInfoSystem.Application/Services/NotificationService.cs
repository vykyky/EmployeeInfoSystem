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

        public static string StatusLabel(string status) => status switch
        {
            "accepted" or "new" => "Принята",
            "assigned" => "Назначена",
            "in_progress" => "В работе",
            "done" => "Выполнена",
            _ => status
        };

        private static string ResponsibleName(Request request) =>
            request.Manager?.EmployeeProfile?.Fio
            ?? request.Manager?.Tabn
            ?? "—";

        private static string TypeName(Request request) =>
            request.RequestType?.Name ?? request.RequestTypeId.ToString();

        private async Task AddInboxAsync(int recipientId, int? senderId, string title, string body)
        {
            await _uow.Notifications.AddAsync(new Notification
            {
                RecipientId = recipientId,
                SenderId = senderId,
                Title = title,
                Body = body,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RequestId = null
            });
        }

        private async Task NotifyEmployeeStatusAsync(Request request, bool includeResponsible)
        {
            var typeName = TypeName(request);
            var statusLabel = StatusLabel(request.Status);
            var body = includeResponsible
                ? $"Электронный запрос «{typeName}» — статус: {statusLabel}. Ответственный: {ResponsibleName(request)}"
                : $"Электронный запрос «{typeName}» — статус: {statusLabel}";

            await AddInboxAsync(
                request.EmployeeId,
                request.ManagerId,
                $"Электронный запрос: {statusLabel}",
                body);
        }

        private async Task NotifyAdminsAsync(int? senderId, string title, string body)
        {
            var admins = await _uow.Users.GetByRolesAsync(new[] { "admin" });
            foreach (var admin in admins)
            {
                await AddInboxAsync(admin.Id, senderId, title, body);
            }
        }

        // ── Внутренние методы (вызываются из RequestService) ─────────────────

        public async Task NotifyNewRequestAsync(Request request)
        {
            // Сотруднику: Принята (тип + статус)
            await NotifyEmployeeStatusAsync(request, includeResponsible: false);

            // Админам: поступление нового электронного запроса
            var typeName = TypeName(request);
            var employee = request.Employee?.EmployeeProfile?.Fio
                ?? request.Employee?.Tabn
                ?? request.EmployeeId.ToString();

            await NotifyAdminsAsync(
                request.EmployeeId,
                "Новый электронный запрос",
                $"Поступил электронный запрос «{typeName}» от {employee}. Статус: {StatusLabel(request.Status)}");
        }

        public async Task NotifyRequestAssignedAsync(Request request)
        {
            // Сотруднику: Назначена (тип + статус + ответственный)
            await NotifyEmployeeStatusAsync(request, includeResponsible: true);

            // Менеджеру: назначен на задачу (админу при самоназначении не дублируем)
            if (request.ManagerId.HasValue && request.Manager?.Role == "manager")
            {
                var typeName = TypeName(request);
                await AddInboxAsync(
                    request.ManagerId.Value,
                    null,
                    "Вам назначена задача",
                    $"Вам назначен электронный запрос «{typeName}» (задача #{request.Id}). Статус: {StatusLabel(request.Status)}");
            }
        }

        public async Task NotifyRequestInProgressAsync(Request request)
        {
            await NotifyEmployeeStatusAsync(request, includeResponsible: true);
        }

        public async Task NotifyRequestResolvedAsync(Request request)
        {
            await NotifyEmployeeStatusAsync(request, includeResponsible: true);

            // Админам: выполнение системных запросов (для последующей синхронизации)
            if (request.RequestType?.IsSystem == true)
            {
                var typeName = TypeName(request);
                await NotifyAdminsAsync(
                    request.ManagerId,
                    "Системный запрос выполнен",
                    $"Системный электронный запрос «{typeName}» выполнен (задача #{request.Id}). Требуется синхронизация данных.");
            }
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
                RequestId = null
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
