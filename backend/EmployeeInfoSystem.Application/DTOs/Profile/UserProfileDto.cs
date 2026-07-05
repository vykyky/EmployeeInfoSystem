using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Profile
{
    public class UserProfileDto
    {
        // Личная информация (из кэша Галактики)
        public string? Fio { get; set; }
        public DateTime? BornDate { get; set; }
        public DateTime? HireDate { get; set; }

        // Локальные настройки пользователя (из приложения)
        public string? Phone { get; set; }
        public string? Email { get; set; }

    }
}
