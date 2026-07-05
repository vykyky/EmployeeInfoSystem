using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.User
{
    public class CreateUserDto
    {
        public string Tabn { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
