using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class Notification
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }           // кому (User)
        public int? SenderId { get; set; }             // от кого (User) или null = система
        public string Title { get; set; }
        public string Body { get; set; }        
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RequestId { get; set; }

        public User? Recipient { get; set; }
        public User? Sender { get; set; }
        public Request? Request { get; set; }
    }
}
