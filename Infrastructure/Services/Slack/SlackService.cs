using Application.Interfaces.Slack;
using Microsoft.Extensions.Configuration;
using Slack.Webhooks;

namespace Infrastructure.Services.Slack
{
    public class SlackService : ISlackService
    {
        private readonly string _webhookUrl;

        public SlackService(IConfiguration configuration)
        {
            _webhookUrl = configuration["Slack:WebhookUrl"] ?? throw new ArgumentNullException("Slack webhook not configured.");
        }

        public async Task SendAlertAsync(string title, string message)
        {
            var slackClient = new SlackClient(_webhookUrl);
            var slackMessage = new SlackMessage
            {
                Text = $"*{title}*\n{message}",
                Username = "AuditAlertBot",
                IconEmoji = ":warning:"
            };

            await slackClient.PostAsync(slackMessage);
        }
    }
}
