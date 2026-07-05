using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Notification
{
    public class SendNotificationDto
    {
        public string Title { get; set; }
        public string Body { get; set; }

        // Указывается одно из двух (либо оба сразу — объединяются)
        public int? RecipientGroupId { get; set; }
        public List<int>? RecipientUserIds { get; set; }
    }
}
