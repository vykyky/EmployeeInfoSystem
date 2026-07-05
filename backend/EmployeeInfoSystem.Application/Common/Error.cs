using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Common
{
    public enum ErrorType
    {
        None,
        NotFound,
        Conflict,
        Validation,
        Unauthorized,
        Forbidden,
        External    // внешний сервис недоступен (Галактика)
    }

   
    public record Error(ErrorType Type, string Message)
    {
        public static readonly Error None = new(ErrorType.None, string.Empty);
        

        public static Error NotFound(string message) => new(ErrorType.NotFound, message);
        public static Error Conflict(string message) => new(ErrorType.Conflict, message);
        public static Error Validation(string message) => new(ErrorType.Validation, message);
        public static Error Unauthorized(string message) => new(ErrorType.Unauthorized, message);
        public static Error Forbidden(string message) => new(ErrorType.Forbidden, message);
        public static Error External(string message) => new(ErrorType.External, message);
    }
}
