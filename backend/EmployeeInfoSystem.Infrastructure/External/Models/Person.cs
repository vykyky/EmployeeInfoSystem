using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.External.Models
{
    public class Person
    {
        public int Nrec { get; set; }
        public string Fio { get; set; }
        public DateTime? BornDate { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
