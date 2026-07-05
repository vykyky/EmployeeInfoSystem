using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class Ppe
    {
        public int Id { get; set; }
        public string Tabn { get; set; }               // кому выдана
        public string? GroupName { get; set; }          // группа (Зимняя/Летняя)
        public string? ItemName { get; set; }           // наименование (Куртка/Ботинки)
        public DateTime? GiveDate { get; set; }         // дата выдачи
        public DateTime? EndDate { get; set; }         // дата окончания носки
        public int? WearPeriod { get; set; }           // срок носки в месяцах (65535 = до износа)
        public decimal? Quantity { get; set; }          // количество
        public DateTime SyncedAt { get; set; }
    }
}
