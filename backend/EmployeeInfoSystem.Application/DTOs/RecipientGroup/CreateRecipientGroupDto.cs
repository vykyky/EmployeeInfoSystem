using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.RecipientGroup
{
    public class CreateRecipientGroupDto
    {
        public string Name { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
    }
}
