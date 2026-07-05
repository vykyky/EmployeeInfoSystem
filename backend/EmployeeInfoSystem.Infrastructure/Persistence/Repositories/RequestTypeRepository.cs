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
    public class RequestTypeRepository : IRequestTypeRepository
    {
        private readonly AppDbContext _db;

        public RequestTypeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RequestType?> GetByIdAsync(int id) =>
            await _db.RequestTypes.FindAsync(id);

        public async Task<IEnumerable<RequestType>> GetAllAsync() =>
            await _db.RequestTypes.OrderBy(t => t.Name).ToListAsync();

        public async Task<List<RequestType>> GetActiveAsync() =>
            await _db.RequestTypes
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

        public async Task AddAsync(RequestType requestType) =>
            await _db.RequestTypes.AddAsync(requestType);

        public async Task UpdateAsync(RequestType requestType) =>
            _db.RequestTypes.Update(requestType);

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.RequestTypes.FindAsync(id);
            if (entity is not null)
                _db.RequestTypes.Remove(entity);
        }

        public async Task<RequestType?> GetByCodeAsync(string code)
        {
            return await _db.RequestTypes
                .FirstOrDefaultAsync(x => x.Code == code && x.IsActive);
        }
    }
}
