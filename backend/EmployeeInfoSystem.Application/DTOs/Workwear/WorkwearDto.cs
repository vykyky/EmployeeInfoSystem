using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Workwear
{
    public class WorkwearDto
    {
        // Размеры из личной карточки (EmployeeProfile)
        public int? ClothesSize { get; set; }
        public int? ShoesSize { get; set; }

        // Список выданной спец. одежды (Ppe)
        public List<PpeItemDto> Items { get; set; } = new();
    }
}
