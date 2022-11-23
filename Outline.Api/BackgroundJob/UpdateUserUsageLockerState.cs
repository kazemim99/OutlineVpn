using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Outline.Api.Database;
using Outline.Api.Services.sms;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Outline.Api.BackgroundJob
{
    public class UpdateUserUsageLockerState : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly IServiceScopeFactory _scopeFactory;

        public UpdateUserUsageLockerState(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        { // remove expired refresh tokens from
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(50));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<DB>();
                //var _smsService = scope.ServiceProvider.GetRequiredService<IRahyabSmsSender>();

                _db.SaveChanges();
            }
            catch (Exception ex) { }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}