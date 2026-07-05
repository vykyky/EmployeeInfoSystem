using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Common
{
    public record Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));

        // Позволяет писать: return someError; вместо return Result.Failure(someError);
        public static implicit operator Result(Error error) => Failure(error);
    }

    // Result<T> — для методов возвращающих данные: Result<int>, Result<UserDto> и т.д.
    public record Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true, null) => Value = value;
        private Result(Error error) : base(false, error) { }

        public static Result<T> Success(T value) => new(value);
        public new static Result<T> Failure(Error error) => new(error);

        // Позволяет писать: return user.Id; вместо return Result<int>.Success(user.Id);
        public static implicit operator Result<T>(T value) => new(value);

        // Позволяет писать: return someError; вместо return Result<T>.Failure(someError);
        public static implicit operator Result<T>(Error error) => new(error);
    }
}
