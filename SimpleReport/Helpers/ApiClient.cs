using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            var user = System.Configuration.ConfigurationManager.AppSettings["ApiUserName"];
            var password = System.Configuration.ConfigurationManager.AppSettings["ApiPassword"];
            var byteArray = Encoding.ASCII.GetBytes(user +":" + password);
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Authorization = header;

            client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}