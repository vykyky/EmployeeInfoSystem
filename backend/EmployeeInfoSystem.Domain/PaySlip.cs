using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class PaySlip
    {
        public int Id { get; set; }
        public string Tabn { get; set; }               // кому принадлежит
        public int Month { get; set; }                 // месяц (1-12)
        public int Year { get; set; }                  // год
        public string FilePath { get; set; }           // путь к HTML файлу
        public DateTime CreatedAt { get; set; }
    }
}
