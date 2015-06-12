using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SimpleReport.Helpers
{
    public interface IApiClient
    {
        Task<object> Get(string url);
        Task<object> Post(string url, object obj);
    }

    public class ApiClient : IApiClient
    {
        public async Task<object> Get(string url)
        {
            using (var client = new HttpClient())
            {
                SetClientSettings(client);

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsAsync<object>();
                return result;
            }
        }

        public async Task<object> Post(string url, object obj)
        {
            using (var client = new HttpClient())
            {
                SetClientSettings(client);

                HttpResponseMessage response = await client.PostAsJsonAsync(url, obj);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsAsync<object>();
                return result;
            }
        }

        private static void SetClientSettings(HttpClient client)
        {
            //TODO: add credentials.
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}