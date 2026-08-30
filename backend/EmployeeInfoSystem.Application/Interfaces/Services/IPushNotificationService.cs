using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs;
using EmployeeInfoSystem.Application.DTOs.PushSubscription;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Application.Interfaces.Services
{
    public interface IPushNotificationService
    {
        Result<string> GetVapidPublicKey();

        Task<Result> SubscribeAsync(int userId, PushSubscriptionDto subscriptionDto, CancellationToken cancellationToken = default);

        Task<Result> UnsubscribeAsync(int userId, string endpoint, CancellationToken cancellationToken = default);

        Task<Result> SendNotificationToUserAsync(int userId, PushNotificationPayload payload, CancellationToken cancellationToken = default);
    }
}