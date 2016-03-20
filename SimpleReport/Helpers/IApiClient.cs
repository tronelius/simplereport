using System.Threading.Tasks;

namespace SimpleReport.Helpers
{
    public interface IApiClient
    {
        Task<object> Get(string url);
        Task<object> Post(string url, object obj);
    }
}