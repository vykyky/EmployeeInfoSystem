using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.News;
using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Application.Interfaces.Repositories;
using EmployeeInfoSystem.Application.Interfaces.Services;
using EmployeeInfoSystem.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Services
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _uow;

        public NewsService(IUnitOfWork uow)  
        {
            _uow = uow;
        }

        public async Task<IEnumerable<NewsDto>> GetAllAsync()
        {
            var news = await _uow.News.GetAllAsync();
            return news.Select(ToDto);
        }

        public async Task<Result<NewsDto>> GetByIdAsync(int id)
        {
            var news = await _uow.News.GetByIdAsync(id);
            if(news == null)
                return Error.NotFound($"Новость {id} не найдена");
            return ToDto(news);
        }

        public async Task<Result<int>> CreateAsync(CreateNewsDto dto, int userId)
        {
            var news = new News
            {
                Title = dto.Title,
                Body = dto.Body,
                ImagePath = dto.ImagePath,
                CreatedAt = DateTime.UtcNow,
                AuthorId = userId,
            };

            await _uow.News.AddAsync(news);
            await _uow.SaveChangesAsync();
            return news.Id;
        }

        public async Task<Result> UpdateAsync(UpdateNewsDto dto)
        {
            var news = await _uow.News.GetByIdAsync(dto.Id);
            if (news is null)
                return Error.NotFound($"Новость {dto.Id} не найдена");

            news.Title = dto.Title;
            news.Body = dto.Body;

            if (dto.ImagePath != null)
                news.ImagePath = dto.ImagePath;

            await _uow.News.UpdateAsync(news);
            await _uow.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var news = await _uow.News.GetByIdAsync(id);
            if (news is null)
                return Error.NotFound($"Новость {id} не найдена");
                
            await _uow.News.DeleteAsync(id);
            await _uow.SaveChangesAsync();
            return Result.Success();
        }

        private static NewsDto ToDto(News n) => new()
        {
            Id = n.Id,
            Title = n.Title,
            Body = n.Body,
            ImagePath = n.ImagePath,
            AuthorId = n.AuthorId,
            CreatedAt = n.CreatedAt
        };
    }
}
