using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IProfileService
    {
        Task<Result<UserProfileDto>> GetProfileByTabnAsync(string tabn);
        Task<Result<int>> RequestContactChangeAsync(string tabn, ChangeContactsRequestDto dto);
    }
}
