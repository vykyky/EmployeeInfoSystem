using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.News
{
    public class NewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string? ImagePath { get; set; }
        public int? AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
