using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.RequestType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IRequestTypeService
    {
        Task<List<RequestTypeDto>> GetAllActiveAsync();   // employee: выпадающий список
        Task<List<RequestTypeDto>> GetAllAsync();          // admin

        Task<Result<int>> CreateAsync(CreateRequestTypeDto dto);
        Task<Result> UpdateAsync(int id, UpdateRequestTypeDto dto);
        Task<Result> DeleteAsync(int id);


        
    }
}
