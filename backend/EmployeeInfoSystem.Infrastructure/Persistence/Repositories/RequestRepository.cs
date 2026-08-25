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
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _db;

        public RequestRepository(AppDbContext db)
        {
            _db = db;
        }

        private IQueryable<Request> WithIncludes() =>
            _db.Requests
                .Include(r => r.Employee).ThenInclude(u => u!.EmployeeProfile)
                .Include(r => r.Manager).ThenInclude(u => u!.EmployeeProfile)
                .Include(r => r.RequestType);

        public async Task<Request?> GetByIdAsync(int id) =>
            await WithIncludes().FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Request>> GetAllAsync() =>
            await WithIncludes()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<List<Request>> GetByEmployeeIdAsync(int employeeId) =>
            await WithIncludes()
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<List<Request>> GetByManagerIdAsync(int managerId) =>
            await WithIncludes()
                .Where(r => r.ManagerId == managerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(Request request) =>
            await _db.Requests.AddAsync(request);

        public async Task UpdateAsync(Request request) =>
            _db.Requests.Update(request);

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Requests.FindAsync(id);
            if (entity is not null)
                _db.Requests.Remove(entity);
        }

        public async Task<bool> ExistsByRequestTypeIdAsync(int requestTypeId) =>
            await _db.Requests.AnyAsync(r => r.RequestTypeId == requestTypeId);
    }

}
