using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Workwear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IWorkwearService
    {
        Task<Result<WorkwearDto>> GetWorkwearByTabnAsync(string tabn);
        Task<Result> RequestSizeChangeAsync(string tabn, ChangeSizesRequestDto dto);
    }
}
