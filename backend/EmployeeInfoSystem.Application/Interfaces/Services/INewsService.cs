using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.News;
using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDto>> GetAllAsync();
        Task<Result<NewsDto>> GetByIdAsync(int id);
        Task<Result<int>> CreateAsync(CreateNewsDto dto, int userId);
        Task <Result> UpdateAsync(UpdateNewsDto dto);
        Task <Result> DeleteAsync(int id);
    }
}
