using V2Ray.Api.Database;
using V2Ray.Api.Services.SSHKeyServices;

namespace V2Ray.Api.BackgroundJob
{

    public class UpdateUserTraffic : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly IServiceScopeFactory _scopeFactory;

        public UpdateUserTraffic(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        { // remove expired refresh tokens from
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(2));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sSHKeyService = scope.ServiceProvider.GetRequiredService<ISSHKeyService>();
                await sSHKeyService.UpdateUserTraffic();

            }
            catch (Exception ex)
            {
            }
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
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(5));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sSHKeyService = scope.ServiceProvider.GetRequiredService<ISSHKeyService>();
                await sSHKeyService.DisableExpired();

            }
            catch (Exception ex)
            {
            }
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


    public class RandomDelete : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly IServiceScopeFactory _scopeFactory;

        public RandomDelete(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        { // remove expired refresh tokens from
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(6));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sSHKeyService = scope.ServiceProvider.GetRequiredService<ISSHKeyService>();
                await sSHKeyService.RandomDel();
            }
            catch (Exception ex)
            {
            }
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