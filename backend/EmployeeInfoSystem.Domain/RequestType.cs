using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Domain
{
    public class RequestType
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public bool IsActive { get; set; }

        public string? Code { get; set; }   // У системных будет "CHANGE_CONTACTS", "CHANGE_SIZES", у пользовательских - null
        public bool IsSystem { get; set; }

    }
}
