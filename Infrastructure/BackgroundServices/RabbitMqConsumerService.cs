using Application.Interfaces.Messaging.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Infrastructure.BackgroundServices
{
    /// <summary>
    /// Background service that runs all registered RabbitMQ consumers
    /// </summary>
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMqConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var consumers = scope.ServiceProvider.GetServices<IRabbitMqConsumer>();

            var tasks = consumers.Select(c => c.ConsumeAsync(stoppingToken));
            await Task.WhenAll(tasks);
        }
    }
}
