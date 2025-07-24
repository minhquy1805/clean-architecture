using Application.DTOs.AuditLogs;
using Application.Interfaces;
using Application.Interfaces.Slack;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Text.Json;
using Application.Interfaces.Redis.Tracking;
using Application.Interfaces.Services;
using System.Diagnostics;
using Infrastructure.Monitoring;
using Application.Interfaces.Messaging.Handlers;


namespace Infrastructure.Messaging.Handlers
{
    public class RabbitMqUserAuditMessageHandler : IRabbitMqUserAuditMessageHandler
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<RabbitMqUserAuditMessageHandler> _logger;
        private readonly IConfiguration _config;

        public RabbitMqUserAuditMessageHandler(
             IServiceProvider provider,
             IConfiguration config,
             ILogger<RabbitMqUserAuditMessageHandler> logger
        )
        {
            _provider = provider;
            _config = config;
            _logger = logger;
        }

        public async Task HandleAsync(string rawMessage)
        {
            using var scope = _provider.CreateScope();

            var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
            var auditTracker = scope.ServiceProvider.GetRequiredService<IAuditZSetTrackerService>();
            var redis = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            var email = scope.ServiceProvider.GetService<IEmailService>();
            var slack = scope.ServiceProvider.GetService<ISlackService>();

            var stopwatch = Stopwatch.StartNew();

            UserAuditMessageDto? dto = null;
            string actionLabel = "Unknown";

            try
            {
                dto = JsonSerializer.Deserialize<UserAuditMessageDto>(rawMessage);
                if (dto == null)
                    throw new Exception("Invalid audit message format.");

                actionLabel = dto.Action ?? "Unknown";

                var entity = new UserAudit
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = dto.UserId,
                    Action = dto.Action!,
                    OldValue = dto.OldValue,
                    NewValue = dto.NewValue,
                    IpAddress = dto.IpAddress,
                    CreatedAt = DateTime.UtcNow,
                    Flag = dto.Flag ?? "T"
                };

                await auditService.LogAuditAsync(entity);
                await auditTracker.TrackActionAsync(entity.Action);
                await auditTracker.TrackActionByDateAsync(entity.Action);

                _logger.LogInformation("✅ Audit log + ZSet OK: {Action}", entity.Action);
                UserAuditMetrics.AuditHandledByAction.WithLabels(actionLabel).Inc();
            }
            catch (Exception ex)
            {
                UserAuditMetrics.AuditFailedByAction
                    .WithLabels(actionLabel, ex.GetType().Name)
                    .Inc();

                await HandleFailureAsync(ex, rawMessage, redis, email, slack);
            }
            finally
            {
                stopwatch.Stop();
                UserAuditMetrics.AuditProcessingDurationByAction
                    .WithLabels(actionLabel)
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }


        private async Task HandleFailureAsync(Exception ex, string rawMessage,
                                          IDistributedCache redis,
                                          IEmailService? email,
                                          ISlackService? slack)
        {
            var adminEmail = _config["MainAdminEmail"];
            var hashKey = $"audit-error:{rawMessage.GetHashCode()}";

            var alreadySent = await redis.GetStringAsync(hashKey);
            if (alreadySent != null)
            {
                _logger.LogWarning("🚫 Duplicate alert suppressed.");
                return;
            }

            await redis.SetStringAsync(hashKey, "sent", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            var alert = $"❌ Lỗi: {ex.Message}\nMessage: {rawMessage}";

            if (email != null && !string.IsNullOrEmpty(adminEmail))
                await email.SendEmailAsync(adminEmail, "🚨 Audit Error", $"<pre>{alert}</pre>");

            if (slack != null)
                await slack.SendAlertAsync("🚨 Audit Error", alert);

            _logger.LogError(ex, "❌ Failed to handle audit message");
        }


    }
}
