using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class RecipientGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }            // например "Все сотрудники ИТ-отдела"
        public string? Department { get; set; }      // фильтр по отделу (из EmployeeProfile.Department)
        public string? Role { get; set; }             // фильтр по роли, либо null = все роли
        public DateTime CreatedAt { get; set; }
    }
}
