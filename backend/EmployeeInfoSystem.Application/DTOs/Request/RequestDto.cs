using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Request
{
    public class RequestDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string? EmployeeTabn { get; set; }     // удобно показывать в списке у менеджера/админа
        public string? EmployeeFio { get; set; }

        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }

        public string? Comment { get; set; }
        public string? NewValue { get; set; }          // новое значение: телефон/email/размер одежды/обуви и т.д.

        public string Status { get; set; }              // "new", "in_progress", "done"

        public int? ManagerId { get; set; }
        public string? ManagerFio { get; set; }

        public string? ResolutionComment { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
