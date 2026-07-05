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
    public class PpeRepository : IPpeRepository
    {
        private readonly AppDbContext _db;
        public PpeRepository(AppDbContext context) { _db = context; }

        public async Task<Ppe?> GetByIdAsync(int id)
        {
            return await _db.Ppes.FindAsync(id);
        }

        public async Task<IEnumerable<Ppe>> GetAllAsync()
        {
            return await _db.Ppes.ToListAsync();
        }

        public async Task AddAsync(Ppe entity)
        {
            await _db.Ppes.AddAsync(entity);

        }

        public Task UpdateAsync(Ppe entity)
        {
            _db.Ppes.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var ppe = await _db.Ppes.FindAsync(id);
            if (ppe != null)
            {
                _db.Ppes.Remove(ppe);
            }
        }

        public async Task<IEnumerable<Ppe>> GetByTabnAsync(string tabn)
        {
            return await _db.Ppes.Where(p => p.Tabn == tabn).ToListAsync();
        }

        public async Task DeleteByTabnAsync(string tabn)
        {
            var items = await _db.Ppes.Where(p => p.Tabn == tabn).ToListAsync();
            _db.Ppes.RemoveRange(items);
        }
    }
}
