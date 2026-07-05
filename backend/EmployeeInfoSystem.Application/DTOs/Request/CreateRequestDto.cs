using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.DTOs.Request
{
    public class CreateRequestDto
    {
        public int RequestTypeId { get; set; }
        public string? Comment { get; set; }

        // Новое значение для запросов на изменение (телефон, email, размер одежды, размер обуви и т.д.)
        // Конкретный смысл определяется RequestTypeId/TargetField — само поле просто хранит строковое значение.
        public string? NewValue { get; set; }

        public string? TargetField { get; set; }
    }
}
