using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Auth
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string Fio { get; set; }
    }
}
