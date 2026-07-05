using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.RequestType
{
    public class CreateRequestTypeDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
