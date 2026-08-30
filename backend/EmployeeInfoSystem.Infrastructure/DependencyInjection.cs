using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Application.Interfaces.Repositories;
using EmployeeInfoSystem.Application.Interfaces.Services;
using EmployeeInfoSystem.Infrastructure.External;
using EmployeeInfoSystem.Infrastructure.Persistence;
using EmployeeInfoSystem.Infrastructure.Persistence.Repositories;
using EmployeeInfoSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace EmployeeInfoSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<GalaktikaDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("GalaktikaConnection")));

            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPpeRepository, PpeRepository>();
            services.AddScoped<IEmployeeProfileRepository, EmployeeProfileRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IRequestTypeRepository, RequestTypeRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IRecipientGroupRepository, RecipientGroupRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ISyncService, SyncService>();
            services.AddScoped<IPushNotificationService, PushNotificationService>();

            services.AddHostedService<SyncBackgroundService>();
            return services;
        }

    }
}
