using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class Request
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }            // кто создал (User)
        public int RequestTypeId { get; set; }         // тип запроса
        public string? Comment { get; set; }           // комментарий
        public string? NewValue { get; set; }          // новое значение (для смены размера и т.п.)
        public string Status { get; set; }             // "accepted", "assigned", "in_progress", "done"
        public int? ManagerId { get; set; }            // кто назначен менеджером
        public string? ResolutionComment { get; set; } // ответ/решение менеджера
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public User? Employee { get; set; }
        public User? Manager { get; set; }
        public RequestType? RequestType { get; set; }
    }
}
