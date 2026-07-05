using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.RecipientGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IRecipientGroupService
    {
        Task<List<RecipientGroupDto>> GetAllAsync();
        Task<Result<int>> CreateAsync(CreateRecipientGroupDto dto);
        Task<Result> DeleteAsync(int id);

        // Разворачивает группу (фильтр по отделу/роли) в список User.Id
        Task<List<int>> ResolveRecipientsAsync(int groupId);
    }
}
