using EmployeeInfoSystem.Application.Interfaces.Repositories;
using EmployeeInfoSystem.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _db;

        public NotificationRepository(AppDbContext db)
        {
            _db = db;
        }

        private IQueryable<Notification> WithIncludes() =>
            _db.Notifications
                .Include(n => n.Sender).ThenInclude(u => u!.EmployeeProfile)
                .Include(n => n.Request).ThenInclude(r => r!.RequestType);

        public async Task<Notification?> GetByIdAsync(int id) =>
            await WithIncludes().FirstOrDefaultAsync(n => n.Id == id);

        public async Task<IEnumerable<Notification>> GetAllAsync() =>
            await WithIncludes()
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<List<Notification>> GetMyNotificationsAsync(int recipientId) =>
            await WithIncludes()
                .Where(n => n.RecipientId == recipientId && n.RequestId == null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<List<Notification>> GetTasksAsync(int recipientId) =>
            await WithIncludes()
                .Where(n => n.RecipientId == recipientId && n.RequestId != null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<List<Notification>> GetAllTasksAsync() =>
            await WithIncludes()
                .Where(n => n.RequestId != null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(Notification notification) =>
            await _db.Notifications.AddAsync(notification);

        public async Task AddRangeAsync(IEnumerable<Notification> notifications) =>
            await _db.Notifications.AddRangeAsync(notifications);

        public async Task UpdateAsync(Notification notification) =>
            _db.Notifications.Update(notification);

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Notifications.FindAsync(id);
            if (entity is not null)
                _db.Notifications.Remove(entity);
        }
    }
}
