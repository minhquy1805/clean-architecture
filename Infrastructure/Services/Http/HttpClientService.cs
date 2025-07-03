using System.Text;


namespace Infrastructure.Services.Http
{
    public class HttpClientService : IHttpClientService
    {
        public async Task<string> GetAsync(string baseUrl, string relativeUrl)
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            var response = await client.GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PostAsync(string baseUrl, string relativeUrl, string jsonBody)
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(relativeUrl, content);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string baseUrl, string relativeUrl)
        {
            using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            var response = await client.DeleteAsync(relativeUrl);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
