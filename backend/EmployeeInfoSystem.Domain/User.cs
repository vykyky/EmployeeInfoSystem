using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeInfoSystem.Domain;

namespace EmployeeInfoSystem.Domain
{
    public class User
    {
        public int Id { get; set; }                    // внутренний ID
        public string Tabn { get; set; }               // табельный номер (логин)
        public string PasswordHash { get; set; }       // хэш пароля
        public string Role { get; set; }               // "employee", "manager", "admin"
        public string? Phone { get; set; }             // для SMS
        public string? Email { get; set; }             // для Email
        public string? PushToken { get; set; }         // для push (FCM/APNs)
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public EmployeeProfile? EmployeeProfile { get; set; }
    }
}
