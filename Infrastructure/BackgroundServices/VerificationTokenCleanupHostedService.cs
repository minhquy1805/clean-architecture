using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Infrastructure.BackgroundServices
{
    public class VerificationTokenCleanupHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<VerificationTokenCleanupHostedService> _logger;

        public VerificationTokenCleanupHostedService(
           IServiceScopeFactory scopeFactory,
           ILogger<VerificationTokenCleanupHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ VerificationTokenCleanupHostedService STARTED.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IUserVerificationRepository>();

                    await repo.DeleteExpiredAsync();

                    _logger.LogInformation("✅ Cleaned up expired verification tokens at {Time}.", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error while cleaning up expired verification tokens.");
                }

                // Wait for 1 hour before running again — adjust as needed!
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("🛑 VerificationTokenCleanupHostedService STOPPED.");
        }
    }
}
