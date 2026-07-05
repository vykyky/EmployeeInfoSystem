using EmployeeInfoSystem.Application.Interfaces.Repositories;
using EmployeeInfoSystem.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        // Все свойства должны быть public, чтобы соответствовать интерфейсу IUnitOfWork
        public INewsRepository News { get; }
        public IUserRepository Users { get; }
        public IEmployeeProfileRepository EmployeeProfiles { get; }
        public IPpeRepository Ppes { get; }
        public IRequestRepository Requests { get; }
        public IRequestTypeRepository RequestTypes { get; }
        public INotificationRepository Notifications { get; }
        public IRecipientGroupRepository RecipientGroups { get; }

        // Полный конструктор, принимающий все зависимости
        public UnitOfWork(
            AppDbContext context,
            INewsRepository news,
            IUserRepository users,
            IEmployeeProfileRepository employeeProfiles,
            IPpeRepository ppes,
            IRequestRepository requests,
            IRequestTypeRepository requestTypes,
            INotificationRepository notifications,
            IRecipientGroupRepository recipientGroups)
        {
            _db = context;
            News = news;
            Users = users;
            EmployeeProfiles = employeeProfiles;
            Ppes = ppes;

            // Дописываем инициализацию оставшихся полей:
            Requests = requests;
            RequestTypes = requestTypes;
            Notifications = notifications;
            RecipientGroups = recipientGroups;
        }

        public Task<int> SaveChangesAsync()
            => _db.SaveChangesAsync();

        public void Dispose()
            => _db.Dispose();
    }
}