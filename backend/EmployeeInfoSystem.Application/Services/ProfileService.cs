using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Profile;
using EmployeeInfoSystem.Application.DTOs.Request;
using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _uow;
        private readonly IRequestService _requestService;
        public ProfileService(IUnitOfWork uow, IRequestService requestService)
        {
            _uow = uow;
            _requestService = requestService;
        }

        public async Task<Result<UserProfileDto>> GetProfileByTabnAsync(string tabn)
        {
            // Получаем пользователя вместе с EmployeeProfile через Include, реализованный в UserRepository
            var user = await _uow.Users.GetByTabnAsync(tabn);
            if (user is null)
                return Error.NotFound($"Пользователь с табельным номером {tabn} не найден");

            var dto = new UserProfileDto
            {
                // Данные из Галактики (EmployeeProfile)
                Fio = user.EmployeeProfile?.Fio,
                BornDate = user.EmployeeProfile?.BornDate,
                HireDate = user.EmployeeProfile?.HireDate,

                // Если в самом приложении User.Phone пуст, показываем дефолтный из Галактики
                Phone = user.Phone ?? user.EmployeeProfile?.Phone,
                Email = user.Email ?? user.EmployeeProfile?.Email,

            };

            return dto;
        }

        // 1. Меняем возвращаемый тип на Result<int>
        public async Task<Result<int>> RequestContactChangeAsync(string tabn, ChangeContactsRequestDto dto)
        {
            var user = await _uow.Users.GetByTabnAsync(tabn);
            if (user is null)
                return Error.NotFound($"Пользователь с табельным номером {tabn} не найден");

            var requestType = await _uow.RequestTypes.GetByCodeAsync("CHANGE_CONTACTS");
            if (requestType is null)
                return Error.NotFound("Системный тип запроса 'CHANGE_CONTACTS' не найден");

            var newValue = $"Телефон: {dto.Phone ?? "(без изменений)"}, Email: {dto.Email ?? "(без изменений)"}";

            // 2. Теперь здесь корректно вернется Result<int>, содержащий ID
            return await _requestService.CreateAsync(user.Id, new CreateRequestDto
            {
                RequestTypeId = requestType.Id,
                NewValue = newValue,
                Comment = "Запрос на изменение контактных данных"
            });
        }
    }
}
