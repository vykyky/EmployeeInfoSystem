using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.External.Models
{
    public class Lschet
    {
        public int Nrec { get; set; }
        public string Tabn { get; set; }
        public int TPerson { get; set; }
        public DateTime? DatPos { get; set; }
        public decimal? Tarif { get; set; }
    }
}
