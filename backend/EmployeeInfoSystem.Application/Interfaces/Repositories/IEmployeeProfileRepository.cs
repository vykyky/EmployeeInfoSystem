using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Repositories
{
    public interface IEmployeeProfileRepository : IRepository<EmployeeProfile>
    {
        Task<EmployeeProfile?> GetByTabnAsync(string tabn);
    }
}
