using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IRequestService
    {
        // employeeId, managerId, requesterId -- это User.Id (внутренний), не tabn
        Task<Result<int>> CreateAsync(int employeeId, CreateRequestDto dto);

        Task<List<RequestDto>> GetMyRequestsAsync(int employeeId);
        Task<List<RequestDto>> GetByManagerIdAsync(int managerId);
        Task<List<RequestDto>> GetAllAsync();   // admin, без фильтра

        Task<Result> TakeInProgressAsync(int requestId, int managerId);
        Task<Result> CompleteAsync(int requestId, int managerId, string resolutionComment);
        Task<Result> AssignManagerAsync(int requestId, int managerId);
    }
}
