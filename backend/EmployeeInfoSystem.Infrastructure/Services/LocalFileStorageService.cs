using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;

        public LocalFileStorageService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"]
                ?? throw new InvalidOperationException("FileStorage:BasePath не задан в конфигурации");
        }

        public async Task<string> SaveAsync(Stream fileStream, string fileName, string subfolder)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(fileName).ToLower();

            if (!allowed.Contains(ext))
                throw new ArgumentException("Недопустимый формат файла");

            var folder = Path.Combine(_basePath, "uploads", subfolder);
            Directory.CreateDirectory(folder);

            var newFileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, newFileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return $"uploads/{subfolder}/{newFileName}";
        }

        public void Delete(string relativePath)
        {
            var fullPath = Path.Combine(_basePath, relativePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
