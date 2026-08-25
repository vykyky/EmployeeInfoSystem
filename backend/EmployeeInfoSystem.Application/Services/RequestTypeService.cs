using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.RequestType;
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
    public class RequestTypeService : IRequestTypeService
    {
        private readonly IUnitOfWork _uow;

        public RequestTypeService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<RequestTypeDto>> GetAllActiveAsync()
        {
            var list = await _uow.RequestTypes.GetActiveAsync();
            return list.Select(ToDto).ToList();
        }

        public async Task<List<RequestTypeDto>> GetAllAsync()
        {
            var list = await _uow.RequestTypes.GetAllAsync();
            return list.Select(ToDto).ToList();
        }

        public async Task<Result<int>> CreateAsync(CreateRequestTypeDto dto)
        {
            var entity = new RequestType
            {
                Name = dto.Name,
                IsActive = dto.IsActive,
                IsSystem = false,
                Code = null
            };

            await _uow.RequestTypes.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Result> UpdateAsync(int id, UpdateRequestTypeDto dto)
        {
            var entity = await _uow.RequestTypes.GetByIdAsync(id);
            if (entity is null)
                return Error.NotFound($"Тип запроса {id} не найден");

            if (entity.IsSystem && !dto.IsActive)
            {
                return Error.Validation("Системные типы запросов нельзя деактивировать, так как к ним привязаны экраны приложения.");
            }

            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive;

            await _uow.RequestTypes.UpdateAsync(entity);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var entity = await _uow.RequestTypes.GetByIdAsync(id);
            if (entity is null)
                return Error.NotFound($"Тип запроса {id} не найден");

            if (entity.IsSystem)
                return Error.Validation("Системные типы запросов нельзя удалять");

            var hasRequests = await _uow.Requests.ExistsByRequestTypeIdAsync(id);
            if (hasRequests)
                return Error.Conflict("Нельзя удалить тип запроса — к нему привязаны существующие запросы. Вместо удаления деактивируйте его через «Изменить».");

            await _uow.RequestTypes.DeleteAsync(id);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        private static RequestTypeDto ToDto(RequestType t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            IsActive = t.IsActive,
            IsSystem = t.IsSystem
        };
    }
}
