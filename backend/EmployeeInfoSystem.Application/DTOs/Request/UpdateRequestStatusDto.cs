using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Request
{
    public class UpdateRequestStatusDto
    {
        public string Status { get; set; }              // "in_progress" или "done"
        public string? ResolutionComment { get; set; }   // обязателен при переводе в "done"
    }
}
