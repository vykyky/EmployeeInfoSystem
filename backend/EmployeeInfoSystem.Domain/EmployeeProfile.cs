using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class EmployeeProfile
    {
        public int Id { get; set; }
        public string Tabn { get; set; }               // связь с User
        public string? Fio { get; set; }          // ФИО
        public DateTime? BornDate { get; set; }        // дата рождения
        public DateTime? HireDate { get; set; }        // дата приёма
        public string? Department { get; set; }        // отдел
        public string? Position { get; set; }          // должность
        public string? Phone { get; set; }             // телефон из Галактики
        public string? Email { get; set; }             // email из Галактики
        public int? ClothesSize { get; set; }          // размер одежды
        public int? WinterClothesSize { get; set; }    // размер зимней одежды
        public int? ShoesSize { get; set; }            // размер обуви
        public int? WinterShoesSize { get; set; }      // размер зимней обуви
        public int? Height { get; set; }               // рост
        public DateTime SyncedAt { get; set; }         // когда синхронизировали
    }
}
