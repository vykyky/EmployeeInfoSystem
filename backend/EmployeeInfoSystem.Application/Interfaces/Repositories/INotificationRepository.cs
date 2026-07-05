using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {

        // RequestId == null -- обычные уведомления сотрудника
        Task<List<Notification>> GetMyNotificationsAsync(int recipientId);

        // RequestId != null -- "задачи" менеджера/админа
        Task<List<Notification>> GetTasksAsync(int recipientId);
        Task<List<Notification>> GetAllTasksAsync();   // для админа, без фильтра по получателю

        Task AddRangeAsync(IEnumerable<Notification> notifications);

    }
}
