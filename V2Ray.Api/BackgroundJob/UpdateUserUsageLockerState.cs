using V2Ray.Api.Database;
using V2Ray.Api.Services.SSHKeyServices;

namespace V2Ray.Api.BackgroundJob
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
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                //using var scope = _scopeFactory.CreateScope();
                //var _db = scope.ServiceProvider.GetRequiredService<DB>();
                //var sSHKeyService = scope.ServiceProvider.GetRequiredService<ISSHKeyService>();
              
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