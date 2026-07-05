using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Profile
{
    public class ChangeContactsRequestDto
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
