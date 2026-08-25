using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Repositories
{
    public interface IRequestRepository : IRepository<Request>
    {
        Task<List<Request>> GetByEmployeeIdAsync(int employeeId);
        Task<List<Request>> GetByManagerIdAsync(int managerId);

        Task<bool> ExistsByRequestTypeIdAsync(int requestTypeId);
    }
}
