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
    public class EmployeeProfileRepository : IEmployeeProfileRepository
    {
        private readonly AppDbContext _db;
        public EmployeeProfileRepository(AppDbContext context) { _db = context; }

        public async Task<EmployeeProfile?> GetByIdAsync(int id)
        {
            return await _db.EmployeeProfiles.FindAsync(id);
        }

        public async Task<IEnumerable<EmployeeProfile>> GetAllAsync()
        {
            return await _db.EmployeeProfiles.ToListAsync();
        }

        public async Task AddAsync(EmployeeProfile entity)
        {
            await _db.EmployeeProfiles.AddAsync(entity);

        }

        public Task UpdateAsync(EmployeeProfile entity)
        {
            _db.EmployeeProfiles.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var ep = await _db.EmployeeProfiles.FindAsync(id);
            if (ep != null)
            {
                _db.EmployeeProfiles.Remove(ep);
            }
        }

        public async Task<EmployeeProfile?> GetByTabnAsync(string tabn)
        {
            return await _db.EmployeeProfiles.FirstOrDefaultAsync(e => e.Tabn == tabn);
        }

    }
}
