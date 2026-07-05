using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByTabnAsync(string tabn);
        Task<List<User>> GetByFilterAsync(string? department, string? role);
        Task<List<User>> GetByRolesAsync(IEnumerable<string> roles);
    }
}
