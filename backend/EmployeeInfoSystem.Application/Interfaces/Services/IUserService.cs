using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<Result<UserDto>> GetByIdAsync(int id);
        Task<Result<int>> CreateAsync(CreateUserDto dto);
        Task <Result> DeleteAsync(int id);
        Task <Result> ChangeRoleAsync(int id, string role);
    }
}
