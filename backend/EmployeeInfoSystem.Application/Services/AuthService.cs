using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs.Auth;
using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;

        public AuthService(IUnitOfWork uow, ITokenService tokenService)
        {
            _uow = uow;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResultDto>> LoginAsync(LoginDto dto)
        {
            var user = await _uow.Users.GetByTabnAsync(dto.Tabn);
            if (user == null)
                return Error.Unauthorized("Неверный табельный номер или пароль");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Error.Unauthorized("Неверный табельный номер или пароль");

            user.LastLoginAt = DateTime.UtcNow;
            await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();

            return new AuthResultDto
            {
                Token = _tokenService.GenerateToken(user),
                Role = user.Role,
                Fio = user.EmployeeProfile?.Fio ?? user.Tabn
            };
        }
    }
}
