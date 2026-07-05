using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResultDto>> LoginAsync(LoginDto dto);
    }
}
