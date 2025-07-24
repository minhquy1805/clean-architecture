using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.HealthChecks
{
    public static class HealthCheckExtension
    {
        public static IServiceCollection AddProjectHealthChecks(this IServiceCollection services, IConfiguration config)
        {
            var builder = services.AddHealthChecks();

            // ✅ SQL Server
            var sqlConn = config["Settings:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(sqlConn))
            {
                builder.AddSqlServer(
                    connectionString: sqlConn,
                    healthQuery: "SELECT 1",
                    name: "sqlserver",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql" });
            }

            // ✅ MongoDB
            var mongoConn = config["MongoDbSettings:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(mongoConn))
            {
                builder.AddMongoDb(
                    mongodbConnectionString: mongoConn,
                    name: "mongodb",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "mongo" });
            }

            // ✅ Redis
            var redisConn = config["Redis:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(redisConn))
            {
                builder.AddRedis(redisConn, name: "redis", tags: new[] { "cache", "redis" });
            }

            // ✅ RabbitMQ
            var rabbitHost = config["RabbitMq:Host"];
            var rabbitPort = config["RabbitMq:Port"];
            var rabbitUser = config["RabbitMq:Username"];
            var rabbitPass = config["RabbitMq:Password"];
            if (!string.IsNullOrWhiteSpace(rabbitHost))
            {
                var rabbitUri = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:{rabbitPort}/";
                builder.AddRabbitMQ(rabbitUri, name: "rabbitmq", tags: new[] { "mq", "rabbitmq" });
            }

            // ✅ Slack (ping webhook)
            builder.AddCheck<SlackWebhookHealthCheck>(
                name: "slack_webhook",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "external", "slack" });

            return services;
        }
    }
}
