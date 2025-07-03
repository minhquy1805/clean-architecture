namespace Application.Common.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public string? Details { get; }

        // Constructor cơ bản
        public AppException(
            string message,
            int statusCode = 400,
            string errorCode = "APP_ERROR",
            string? details = null
        ) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
        }

        // Constructor có innerException (ví dụ: khi muốn throw từ một Exception khác)
        public AppException(
            string message,
            Exception innerException,
            int statusCode = 400,
            string errorCode = "APP_ERROR",
            string? details = null
        ) : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
