using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.User;
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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly ISyncService _syncService;

        public UserService(IUnitOfWork uow, ISyncService syncService)
        {
            _uow = uow;
            _syncService = syncService;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _uow.Users.GetAllAsync();
            return users.Select(ToDto);
        }

        public async Task<Result<UserDto>> GetByIdAsync(int id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user is null)
                return Error.NotFound($"Пользователь {id} не найден");

            return ToDto(user);
        }

        public async Task<Result<int>> CreateAsync(CreateUserDto dto)
        {
            // проверяем что tabn существует в Галактике
            var existsInGalaxy = await _syncService.TabnExistsAsync(dto.Tabn);
            if (!existsInGalaxy)
                return Error.NotFound($"Табельный номер {dto.Tabn} не найден в Галактике");

            // проверяем что не зарегистрирован
            var existing = await _uow.Users.GetByTabnAsync(dto.Tabn);
            if (existing != null)
                return Error.Conflict($"Пользователь с табельным номером {dto.Tabn} уже существует");

            var user = new User
            {
                Tabn = dto.Tabn,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            // сразу синхронизируем профиль
            await _syncService.SyncEmployeeByTabnAsync(dto.Tabn);

            // implicit operator: int → Result<int>
            return user.Id;
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user is null)
                return Error.NotFound($"Пользователь {id} не найден");
                
            await _uow.Users.DeleteAsync(id);
            await _uow.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> ChangeRoleAsync(int id, string role)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user is null)
                return Error.NotFound($"Пользователь {id} не найден");
               
            user.Role = role;
            await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();
            return Result.Success();
        }

        //нужно еще сервисы потом для обновления телефона (вроде пользователь админу этот запрос будет кидать)!!!
        private static UserDto ToDto(User u) => new()
        {
            Id = u.Id,
            Tabn = u.Tabn,
            Role = u.Role,
            Fio = u.EmployeeProfile?.Fio,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        };
    }
}
