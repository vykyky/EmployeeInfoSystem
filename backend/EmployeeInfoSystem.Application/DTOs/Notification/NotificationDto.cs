using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public int RecipientId { get; set; }
        public int? SenderId { get; set; }
        public string? SenderFio { get; set; }          // null = системное уведомление

        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Если уведомление связано с задачей — заполнено, и фронт может перейти к самой заявке
        public int? RequestId { get; set; }
        public string? RequestStatus { get; set; }       // "new" / "in_progress" / "done", только если RequestId != null
    }
}
