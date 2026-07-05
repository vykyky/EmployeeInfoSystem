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
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _db;
        public NewsRepository(AppDbContext context) { _db = context; }

        public async Task<News?> GetByIdAsync(int id)
        {
            return await _db.News.FindAsync(id);
        }

        public async Task<IEnumerable<News>> GetAllAsync()
        {
            return await _db.News.ToListAsync();
        }

        public async Task AddAsync(News entity)
        {
            await _db.News.AddAsync(entity);
         
        }

        public Task UpdateAsync(News entity)
        { 
            _db.News.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var news = await _db.News.FindAsync(id);
            if (news != null)
            {
                _db.News.Remove(news);
            }
        }
    }
}
