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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext context) { _db = context; }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users
                .Include(u => u.EmployeeProfile)
                .ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _db.Users.AddAsync(entity);

        }

        public Task UpdateAsync(User entity)
        {
            _db.Users.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                _db.Users.Remove(user);
            }
        }

        public async Task<User?> GetByTabnAsync(string tabn) 
        {
            return await _db.Users
                .Include(u => u.EmployeeProfile)
                .FirstOrDefaultAsync(u => u.Tabn == tabn);
        }

        public async Task<List<User>> GetByFilterAsync(string? department, string? role)
        {
            var query = _db.Users
                .Include(u => u.EmployeeProfile)
                .AsQueryable();

            if (!string.IsNullOrEmpty(department))
                query = query.Where(u =>
                    u.EmployeeProfile != null &&
                    u.EmployeeProfile.Department == department);

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);

            return await query.ToListAsync();
        }

        public async Task<List<User>> GetByRolesAsync(IEnumerable<string> roles) =>
            await _db.Users
                .Include(u => u.EmployeeProfile)
                .Where(u => roles.Contains(u.Role))
                .ToListAsync();
    }
}
