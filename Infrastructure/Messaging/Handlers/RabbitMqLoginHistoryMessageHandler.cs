using Application.DTOs.LoginHistories;
using Application.Interfaces;
using Application.Interfaces.Messaging.Handlers;
using Application.Interfaces.Redis.Caching;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Slack;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Diagnostics;
using System.Text.Json;

namespace Infrastructure.Messaging.Handlers
{
    public class RabbitMqLoginHistoryMessageHandler : IRabbitMqLoginHistoryMessageHandler
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<RabbitMqLoginHistoryMessageHandler> _logger;
        private readonly IConfiguration _config;

        public RabbitMqLoginHistoryMessageHandler(
           IServiceProvider provider,
           IConfiguration config,
           ILogger<RabbitMqLoginHistoryMessageHandler> logger)
        {
            _provider = provider;
            _config = config;
            _logger = logger;
        }

        public async Task HandleAsync(string rawMessage)
        {
            using var scope = _provider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<ILoginHistoryRepository>();
            var redis = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            var email = scope.ServiceProvider.GetService<IEmailService>();
            var slack = scope.ServiceProvider.GetService<ISlackService>();
            var loginHistoryService = scope.ServiceProvider.GetRequiredService<ILoginHistoryService>();
            var cacheService = scope.ServiceProvider.GetRequiredService<ILoginHistoryCacheService>();

            string actionLabel = "Unknown";
            Stopwatch? stopwatch = null;

            try
            {
                stopwatch = Stopwatch.StartNew();

                var dto = JsonSerializer.Deserialize<LoginHistoryMessageDto>(rawMessage);
                if (dto == null)
                    throw new Exception("Invalid login history message format.");

                actionLabel = dto.IsSuccess ? "Success" : "Failed";

                var entity = new LoginHistory
                {
                    LoginHistoryId = ObjectId.GenerateNewId().ToString(),
                    UserId = dto.UserId,
                    IsSuccess = dto.IsSuccess,
                    IpAddress = dto.IpAddress ?? string.Empty,
                    UserAgent = dto.UserAgent ?? string.Empty,
                    Message = dto.Message,
                    Device = dto.Device ?? string.Empty,
                    OS = dto.OS ?? string.Empty,
                    Browser = dto.Browser ?? string.Empty,
                    CreatedAt = dto.CreatedAt,
                    Flag = "T"
                };

                await repository.InsertAsync(entity);

                _logger.LogInformation("✅ Login history inserted for UserId {UserId}, Success={Success}", dto.UserId, dto.IsSuccess);


                // Mapping from message DTO → cache DTO
                var cacheDto = new LoginHistoryDto
                {
                    UserId = dto.UserId,
                    IsSuccess = dto.IsSuccess,
                    IpAddress = dto.IpAddress!,
                    CreatedAt = dto.CreatedAt,
                    Device = dto.Device!,
                    Browser = dto.Browser!,
                    UserAgent = dto.UserAgent!,
                    OS = dto.OS!,
                    Message = dto.Message,
                };

                // Update Redis cache
                await cacheService.SetLastLoginAsync(dto.UserId, cacheDto);

                _logger.LogInformation("✅ Login history inserted for UserId {UserId}, Success={Success}", dto.UserId, dto.IsSuccess);
                // LoginHistoryMetrics.LoginHandledByStatus.WithLabels(actionLabel).Inc(); // (Tùy chọn: metrics)
            }
            catch (Exception ex)
            {
                // LoginHistoryMetrics.LoginFailedByStatus.WithLabels(actionLabel, ex.GetType().Name).Inc();
                await HandleFailureAsync(ex, rawMessage, redis, email, slack);
            }
            finally
            {
                stopwatch?.Stop();
                // LoginHistoryMetrics.LoginDurationByStatus.WithLabels(actionLabel).Observe(stopwatch.Elapsed.TotalSeconds);

                _logger.LogInformation("⏱️ [LoginHistory] Processing time: {Seconds}s", stopwatch!.Elapsed.TotalSeconds);
            }
        }

        private async Task HandleFailureAsync(Exception ex, string rawMessage,
                                              IDistributedCache redis,
                                              IEmailService? email,
                                              ISlackService? slack)
        {
            var adminEmail = _config["MainAdminEmail"];
            var hashKey = $"loginhistory-error:{rawMessage.GetHashCode()}";

            var alreadySent = await redis.GetStringAsync(hashKey);
            if (alreadySent != null)
            {
                _logger.LogWarning("🚫 Duplicate login history alert suppressed.");
                return;
            }

            await redis.SetStringAsync(hashKey, "sent", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            var alert = $"❌ Lỗi ghi LoginHistory: {ex.Message}\nMessage: {rawMessage}";

            if (email != null && !string.IsNullOrEmpty(adminEmail))
                await email.SendEmailAsync(adminEmail, "🚨 LoginHistory Error", $"<pre>{alert}</pre>");

            if (slack != null)
                await slack.SendAlertAsync("🚨 LoginHistory Error", alert);

            _logger.LogError(ex, "❌ Failed to handle login history message");
        }
    }
}
