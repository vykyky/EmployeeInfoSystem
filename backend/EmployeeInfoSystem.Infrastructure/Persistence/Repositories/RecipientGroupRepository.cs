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
    public class RecipientGroupRepository : IRecipientGroupRepository
    {
        private readonly AppDbContext _db;

        public RecipientGroupRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RecipientGroup?> GetByIdAsync(int id) =>
            await _db.RecipientGroups.FindAsync(id);

        public async Task<IEnumerable<RecipientGroup>> GetAllAsync() =>
            await _db.RecipientGroups.OrderBy(g => g.Name).ToListAsync();

        public async Task AddAsync(RecipientGroup group) =>
            await _db.RecipientGroups.AddAsync(group);

        public async Task UpdateAsync(RecipientGroup group) =>
            _db.RecipientGroups.Update(group);

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.RecipientGroups.FindAsync(id);
            if (entity is not null)
                _db.RecipientGroups.Remove(entity);
        }
    }
}
