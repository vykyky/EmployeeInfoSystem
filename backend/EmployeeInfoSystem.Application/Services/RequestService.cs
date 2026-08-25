using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Request;
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
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _uow;
        private readonly INotificationService _notificationService;

        public RequestService(IUnitOfWork uow, INotificationService notificationService)
        {
            _uow = uow;
            _notificationService = notificationService;
        }

        public async Task<Result<int>> CreateAsync(int employeeId, CreateRequestDto dto)
        {
            var requestType = await _uow.RequestTypes.GetByIdAsync(dto.RequestTypeId);
            if (requestType is null)
                return Error.NotFound($"Тип запроса {dto.RequestTypeId} не найден");

            if (!requestType.IsActive)
                return Error.Validation("Данный тип запроса больше не доступен");

            var request = new Request
            {
                EmployeeId = employeeId,
                RequestTypeId = dto.RequestTypeId,
                Comment = dto.Comment,
                NewValue = dto.NewValue,
                Status = "accepted",
                ManagerId = null,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Requests.AddAsync(request);
            await _uow.SaveChangesAsync();

            var saved = await _uow.Requests.GetByIdAsync(request.Id);
            if (saved is not null)
            {
                await _notificationService.NotifyNewRequestAsync(saved);
                await _uow.SaveChangesAsync();
            }

            return request.Id;
        }

        public async Task<List<RequestDto>> GetMyRequestsAsync(int employeeId)
        {
            var list = await _uow.Requests.GetByEmployeeIdAsync(employeeId);
            return list.Select(ToDto).ToList();
        }

        public async Task<List<RequestDto>> GetByManagerIdAsync(int managerId)
        {
            var list = await _uow.Requests.GetByManagerIdAsync(managerId);
            return list.Select(ToDto).ToList();
        }

        public async Task<List<RequestDto>> GetAllAsync()
        {
            var list = await _uow.Requests.GetAllAsync();
            return list.Select(ToDto).ToList();
        }

        public async Task<Result> TakeInProgressAsync(int requestId, int managerId)
        {
            var request = await _uow.Requests.GetByIdAsync(requestId);
            if (request is null)
                return Error.NotFound($"Запрос {requestId} не найден");

            if (request.ManagerId != managerId)
                return Error.Forbidden("Запрос назначен другому менеджеру");

            // assigned — основной статус; new — совместимость со старыми записями
            if (request.Status != "assigned" && request.Status != "new")
                return Error.Conflict("Запрос ещё не назначен или уже в работе / завершён");

            request.Status = "in_progress";
            await _uow.Requests.UpdateAsync(request);
            await _uow.SaveChangesAsync();

            var saved = await _uow.Requests.GetByIdAsync(requestId);
            if (saved is not null)
            {
                await _notificationService.NotifyRequestInProgressAsync(saved);
                await _uow.SaveChangesAsync();
            }

            return Result.Success();
        }

        public async Task<Result> CompleteAsync(int requestId, int managerId, string resolutionComment)
        {
            var request = await _uow.Requests.GetByIdAsync(requestId);
            if (request is null)
                return Error.NotFound($"Запрос {requestId} не найден");

            if (request.ManagerId != managerId)
                return Error.Forbidden("Запрос назначен другому менеджеру");

            if (request.Status == "done")
                return Error.Conflict("Запрос уже завершён");

            request.Status = "done";
            request.ResolutionComment = resolutionComment;
            request.ResolvedAt = DateTime.UtcNow;

            await _uow.Requests.UpdateAsync(request);
            await _uow.SaveChangesAsync();

            var saved = await _uow.Requests.GetByIdAsync(requestId);
            if (saved is not null)
            {
                await _notificationService.NotifyRequestResolvedAsync(saved);
                await _uow.SaveChangesAsync();
            }

            return Result.Success();
        }

        public async Task<Result> AssignManagerAsync(int requestId, int managerId)
        {
            var request = await _uow.Requests.GetByIdAsync(requestId);
            if (request is null)
                return Error.NotFound($"Запрос {requestId} не найден");

            if (request.Status == "done")
                return Error.Conflict("Нельзя назначить ответственного на завершённый запрос");

            var manager = await _uow.Users.GetByIdAsync(managerId);
            if (manager is null || (manager.Role != "manager" && manager.Role != "admin"))
                return Error.Validation("Указанный пользователь не является менеджером или администратором");

            request.ManagerId = managerId;

            // При первичном назначении / переназначении из «Принята» — статус «Назначена»
            if (request.Status == "accepted" || request.Status == "new" || request.Status == "assigned")
                request.Status = "assigned";

            await _uow.Requests.UpdateAsync(request);
            await _uow.SaveChangesAsync();

            var saved = await _uow.Requests.GetByIdAsync(requestId);
            if (saved is not null)
            {
                await _notificationService.NotifyRequestAssignedAsync(saved);
                await _uow.SaveChangesAsync();
            }

            return Result.Success();
        }

        private static RequestDto ToDto(Request r) => new()
        {
            Id = r.Id,
            EmployeeId = r.EmployeeId,
            EmployeeTabn = r.Employee?.Tabn,
            EmployeeFio = r.Employee?.EmployeeProfile?.Fio,
            RequestTypeId = r.RequestTypeId,
            RequestTypeName = r.RequestType?.Name ?? string.Empty,
            Comment = r.Comment,
            NewValue = r.NewValue,
            Status = r.Status,
            ManagerId = r.ManagerId,
            ManagerFio = r.Manager?.EmployeeProfile?.Fio ?? r.Manager?.Tabn,
            ResolutionComment = r.ResolutionComment,
            CreatedAt = r.CreatedAt,
            ResolvedAt = r.ResolvedAt
        };
    }
}
