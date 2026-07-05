using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Workwear
{
    public class PpeItemDto
    {
        public string? GroupName { get; set; }   // группа (Зимняя/Летняя)
        public string? ItemName { get; set; }    // наименование (Куртка/Ботинки)
        public DateTime? GiveDate { get; set; }  // дата выдачи
        public DateTime? EndDate { get; set; }   // дата окончания носки
        public decimal? Quantity { get; set; }   // количество
        public int? WearPeriod { get; set; }     // срок носки в месяцах (65535 = до износа)
    }
}
