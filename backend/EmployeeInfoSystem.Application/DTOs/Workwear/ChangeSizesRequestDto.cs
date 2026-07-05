using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Workwear
{
    public class ChangeSizesRequestDto
    {
        public string? ClothesSize { get; set; } // Принимаем как строку, сервис сам разберется
        public string? ShoesSize { get; set; }
    }
}
