using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;

namespace Infrastructure.HealthChecks
{
    public class SlackWebhookHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public SlackWebhookHealthCheck(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
           HealthCheckContext context,
           CancellationToken cancellationToken = default)
        {
            var webhookUrl = _config["Slack:WebhookUrl"];
            if (string.IsNullOrWhiteSpace(webhookUrl))
                return HealthCheckResult.Unhealthy("Slack webhook URL is not configured.");

            var httpClient = _httpClientFactory.CreateClient();

            var payload = new
            {
                text = $"✅ Slack HealthCheck ping from Commercial News at {DateTime.UtcNow:O}"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await httpClient.PostAsync(webhookUrl, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy("Slack webhook is working.");

                return HealthCheckResult.Unhealthy($"Slack webhook returned {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Slack webhook exception: {ex.Message}");
            }
        }
    }
}
