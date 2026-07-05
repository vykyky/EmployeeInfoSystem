using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface ISyncService
    {
        Task<bool> TabnExistsAsync(string tabn);
        Task SyncAllAsync();
        Task SyncEmployeeByTabnAsync(string tabn);

        Task SyncAllProfilesAsync();
        Task SyncAllPpeAsync();
        Task SyncProfileByTabnAsync(string tabn);
        Task SyncPpeByTabnAsync(string tabn);
    }
}
