using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EmployeeInfoSystem.Application.Interfaces.Services;
using EmployeeInfoSystem.Application.Services;

namespace EmployeeInfoSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IWorkwearService, WorkwearService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IRequestTypeService, RequestTypeService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRecipientGroupService, RecipientGroupService>();
            return services;
        }
    }
}
