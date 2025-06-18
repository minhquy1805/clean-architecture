using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IHttpClientService
    {
        Task<string> GetAsync(string baseUrl, string relativeUrl);
        Task<HttpResponseMessage> PostAsync(string baseUrl, string relativeUrl, string jsonBody);
        Task<HttpResponseMessage> DeleteAsync(string baseUrl, string relativeUrl);
    }
}
