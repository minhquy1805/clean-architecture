

namespace Infrastructure.Services.Http
{
    public interface IHttpClientService
    {
        Task<string> GetAsync(string baseUrl, string relativeUrl);
        Task<HttpResponseMessage> PostAsync(string baseUrl, string relativeUrl, string jsonBody);
        Task<HttpResponseMessage> DeleteAsync(string baseUrl, string relativeUrl);
    }
}
