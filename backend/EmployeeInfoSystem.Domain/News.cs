using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string? ImagePath { get; set; }         // путь к картинке
        public int? AuthorId { get; set; }              // ID менеджера из User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? Author { get; set; }
    }
}
