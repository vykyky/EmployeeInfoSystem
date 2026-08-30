using EmployeeInfoSystem.Application.Common;
using EmployeeInfoSystem.Application.DTOs;
using EmployeeInfoSystem.Application.DTOs.PushSubscription;
using EmployeeInfoSystem.Application.Interfaces.Services;
using EmployeeInfoSystem.Domain;
using EmployeeInfoSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebPush;

namespace EmployeeInfoSystem.Infrastructure.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly AppDbContext _context;
        private readonly VapidDetails _vapidDetails;
        private readonly string _publicKey;

        public PushNotificationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;

            var subject = configuration["VapidSettings:Subject"];
            _publicKey = configuration["VapidSettings:PublicKey"] ?? string.Empty;
            var privateKey = configuration["VapidSettings:PrivateKey"];

            _vapidDetails = new VapidDetails(subject, _publicKey, privateKey);
        }

        public Result<string> GetVapidPublicKey()
        {
            if (string.IsNullOrEmpty(_publicKey))
            {
                return Result<string>.Failure(Error.Validation("VAPID публичный ключ не настроен"));
            }

            return Result<string>.Success(_publicKey);
        }

        public async Task<Result> SubscribeAsync(int userId, PushSubscriptionDto subscriptionDto, CancellationToken cancellationToken = default)
        {
            if (subscriptionDto == null || string.IsNullOrWhiteSpace(subscriptionDto.Endpoint))
            {
                return Result.Failure(Error.Validation("Некорректные данные подписки"));
            }

            var existingSubscription = await _context.PushSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Endpoint == subscriptionDto.Endpoint, cancellationToken);

            if (existingSubscription == null)
            {
                var newSubscription = new PushSubscriptionEntity
                {
                    UserId = userId,
                    Endpoint = subscriptionDto.Endpoint,
                    P256dh = subscriptionDto.Keys.P256dh,
                    Auth = subscriptionDto.Keys.Auth,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PushSubscriptions.Add(newSubscription);
            }
            else
            {
                existingSubscription.P256dh = subscriptionDto.Keys.P256dh;
                existingSubscription.Auth = subscriptionDto.Keys.Auth;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> UnsubscribeAsync(int userId, string endpoint, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                return Result.Failure(Error.Validation("Endpoint не может быть пустым"));
            }

            var subscription = await _context.PushSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Endpoint == endpoint, cancellationToken);

            if (subscription == null)
            {
                return Result.Failure(Error.NotFound("Подписка не найдена"));
            }

            _context.PushSubscriptions.Remove(subscription);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> SendNotificationToUserAsync(int userId, PushNotificationPayload payload, CancellationToken cancellationToken = default)
        {
            var subscriptions = await _context.PushSubscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync(cancellationToken);

            if (!subscriptions.Any())
            {
                return Result.Failure(Error.NotFound("У пользователя нет активных подписок"));
            }

            var webPushClient = new WebPushClient();
            var jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            foreach (var sub in subscriptions)
            {
                var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);

                try
                {
                    await webPushClient.SendNotificationAsync(pushSubscription, jsonPayload, _vapidDetails, cancellationToken);
                }
                catch (WebPushException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Gone || ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _context.PushSubscriptions.Remove(sub);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка отправки push-уведомления: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}