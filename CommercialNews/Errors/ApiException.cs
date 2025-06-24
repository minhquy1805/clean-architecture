namespace CommercialNews.Errors
{
    public class ApiException
    {
        public int StatusCode { get; }
        public string Message { get; }
        public string? Details { get; }

        public ApiException(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
