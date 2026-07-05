using EmployeeInfoSystem.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INewsRepository News { get; }
        IUserRepository Users { get; }
        IEmployeeProfileRepository EmployeeProfiles { get; }
        IPpeRepository Ppes { get; }

        IRequestRepository Requests { get; }
        IRequestTypeRepository RequestTypes { get; }
        INotificationRepository Notifications { get; }
        IRecipientGroupRepository RecipientGroups { get; }

        Task<int> SaveChangesAsync();
    }
}
