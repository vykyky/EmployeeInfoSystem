using EmployeeInfoSystem.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.Services
{
    public class SyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SyncBackgroundService> _logger;

        // Время запуска — 02:00 ночи
        private readonly TimeOnly _runAt = new(2, 0);

        public SyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<SyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DailySyncBackgroundService запущен. Синхронизация будет выполняться каждый день в {Time}.", _runAt);

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun();
                _logger.LogInformation("Следующая синхронизация через {Delay}.", delay);

                await Task.Delay(delay, stoppingToken);

                await RunSyncAsync(stoppingToken);
            }
        }

        private async Task RunSyncAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Запуск ежедневной синхронизации с Галактикой...");
            try
            {
                // SyncService — Scoped, поэтому создаём отдельный scope
                using var scope = _scopeFactory.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

                await syncService.SyncAllAsync();

                _logger.LogInformation("Ежедневная синхронизация завершена успешно.");
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Ошибка во время ежедневной синхронизации с Галактикой.");
            }
        }

        private TimeSpan GetDelayUntilNextRun()
        {
            var now = DateTime.Now;
            var nextRun = now.Date.Add(_runAt.ToTimeSpan());

            // Если сегодняшнее время уже прошло — запускаем завтра
            if (nextRun <= now)
                nextRun = nextRun.AddDays(1);

            return nextRun - now;
        }
    }
}
