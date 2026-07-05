using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.External.Models
{
    public class PersSfo
    {
        public int Nrec { get; set; }
        public int CPerscard { get; set; }
        public int CGrupSfo { get; set; }
        public int CKatMbp { get; set; }
        public decimal? CurKol { get; set; }
        public int? Spisdate { get; set; }
        public DateTime? GiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Srok { get; set; }
    }
}
