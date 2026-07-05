using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.RecipientGroup;
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
    public class RecipientGroupService : IRecipientGroupService
    {
        private readonly IUnitOfWork _uow;

        public RecipientGroupService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<RecipientGroupDto>> GetAllAsync()
        {
            var list = await _uow.RecipientGroups.GetAllAsync();
            return list.Select(ToDto).ToList();   // IEnumerable<T> — ToList() работает нормально
        }

        public async Task<Result<int>> CreateAsync(CreateRecipientGroupDto dto)
        {
            var entity = new RecipientGroup
            {
                Name = dto.Name,
                Department = dto.Department,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.RecipientGroups.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var entity = await _uow.RecipientGroups.GetByIdAsync(id);
            if (entity is null)
                return Error.NotFound($"Группа получателей {id} не найдена");

            await _uow.RecipientGroups.DeleteAsync(id);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<List<int>> ResolveRecipientsAsync(int groupId)
        {
            var group = await _uow.RecipientGroups.GetByIdAsync(groupId);
            if (group is null) return new List<int>();

            var users = await _uow.Users.GetByFilterAsync(group.Department, group.Role);
            return users.Select(u => u.Id).ToList();
        }

        private static RecipientGroupDto ToDto(RecipientGroup g) => new()
        {
            Id = g.Id,
            Name = g.Name,
            Department = g.Department,
            Role = g.Role,
            CreatedAt = g.CreatedAt
        };
    }
}
