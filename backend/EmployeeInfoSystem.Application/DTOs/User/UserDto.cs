using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Tabn { get; set; }
        public string Role { get; set; }
        public string? Fio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
