using Prometheus;

namespace Infrastructure.Monitoring
{
    public class LoginHistoryMetrics
    {
        public static readonly Counter LoginHandledByAction = Metrics.CreateCounter(
            "login_history_handled_total", "Tổng số login message xử lý thành công theo action",
            new CounterConfiguration
            {
                LabelNames = new[] { "action" }
            });

        public static readonly Counter LoginFailedByAction = Metrics.CreateCounter(
             "login_history_failed_total", "Tổng số login message lỗi theo action và error_type",
             new CounterConfiguration
             {
                 LabelNames = new[] { "action", "error_type" }
             });

        public static readonly Histogram LoginProcessingDurationByAction = Metrics.CreateHistogram(
            "login_history_processing_duration_seconds_by_action",
            "Thời gian xử lý mỗi login message theo action",
            new HistogramConfiguration
            {
                LabelNames = new[] { "action" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 10) // từ 1ms tới ~500ms
            });
    }
}
