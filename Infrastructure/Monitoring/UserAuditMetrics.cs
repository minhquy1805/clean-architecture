using Prometheus;

namespace Infrastructure.Monitoring
{
    public static class UserAuditMetrics
    {
        public static readonly Counter AuditHandledByAction = Metrics.CreateCounter(
            "user_audit_handled_total", "Tổng số message xử lý thành công theo action",
            new CounterConfiguration
            {
                LabelNames = new[] { "action" }
            });

        public static readonly Counter AuditFailedByAction = Metrics.CreateCounter(
             "user_audit_failed_total", "Tổng số message lỗi theo action và error_type",
             new CounterConfiguration
             {
                 LabelNames = new[] { "action", "error_type" }
             });


        public static readonly Histogram AuditProcessingDurationByAction = Metrics.CreateHistogram(
            "user_audit_processing_duration_seconds_by_action",
            "Thời gian xử lý mỗi message theo action",
            new HistogramConfiguration
            {
                LabelNames = new[] { "action" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 10) // từ 1ms tới ~500ms
            });
    }
}
