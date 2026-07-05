using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.External.Models
{
    public class GroupSfo
    {
        public int Nrec { get; set; }
        public string Name { get; set; }    // наименование группы (Летняя/Зимняя)
        public string? Kod { get; set; }    // код группы
        public int[]? Positions { get; set; }
    }
}
