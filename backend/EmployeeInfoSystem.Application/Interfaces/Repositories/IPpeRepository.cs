using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Repositories
{
    public interface IPpeRepository : IRepository<Ppe>
    {
        Task DeleteByTabnAsync(string tabn);
        Task<IEnumerable<Ppe>> GetByTabnAsync(string tabn);
    }
}
