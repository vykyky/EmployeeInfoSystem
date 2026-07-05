using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Request;
using EmployeeInfoSystem.Application.DTOs.Workwear;
using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Services
{
    public class WorkwearService : IWorkwearService
    {
        private readonly IUnitOfWork _uow;
        private readonly IRequestService _requestService;

        public WorkwearService(IUnitOfWork uow, IRequestService requestService)
        {
            _uow = uow;
            _requestService = requestService;
        }

        public async Task<Result<WorkwearDto>> GetWorkwearByTabnAsync(string tabn)
        {
            var user = await _uow.Users.GetByTabnAsync(tabn);
            if (user is null)
                return Error.NotFound($"Пользователь с табельным номером {tabn} не найден");

            var ppeList = await _uow.Ppes.GetByTabnAsync(tabn);

            var dto = new WorkwearDto
            {
                ClothesSize = user.EmployeeProfile?.ClothesSize,
                ShoesSize = user.EmployeeProfile?.ShoesSize,

                // Список выданной спец. одежды, отмаппленный в PpeItemDto
                Items = ppeList.Select(p => new PpeItemDto
                {
                    GroupName = p.GroupName,
                    ItemName = p.ItemName,
                    GiveDate = p.GiveDate,
                    EndDate = p.EndDate,
                    Quantity = p.Quantity,
                    WearPeriod = p.WearPeriod
                }).ToList()

            };

            return dto;
        }

        public async Task<Result> RequestSizeChangeAsync(string tabn, ChangeSizesRequestDto dto)
        {
            var user = await _uow.Users.GetByTabnAsync(tabn);
            if (user is null)
                return Error.NotFound($"Пользователь с табельным номером {tabn} не найден");

            // Сами ищем тип по коду
            var requestType = await _uow.RequestTypes.GetByCodeAsync("CHANGE_SIZES");
            if (requestType is null)
                return Error.NotFound("Системный тип запроса 'CHANGE_SIZES' не найден");

            // Вся логика парсинга строк в числа теперь скрыта здесь, внутри сервиса!
            int? clothesSize = int.TryParse(dto.ClothesSize, out var c) ? c : null;
            int? shoesSize = int.TryParse(dto.ShoesSize, out var s) ? s : null;

            var newValue = $"Одежда: {clothesSize?.ToString() ?? "(без изменений)"}, Обувь: {shoesSize?.ToString() ?? "(без изменений)"}";

            return await _requestService.CreateAsync(user.Id, new CreateRequestDto
            {
                RequestTypeId = requestType.Id,
                NewValue = newValue,
                Comment = "Запрос на изменение размеров спецодежды"
            });
        }
    }
}
