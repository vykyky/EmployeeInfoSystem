using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class PushSubscriptionEntity
    {
        public int Id { get; set; }

        // Внешний ключ на твоего User (числовой int)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Поля подписки браузера Web Push
        public string Endpoint { get; set; } = string.Empty;
        public string P256dh { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
