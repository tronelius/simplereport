using System;
using System.Configuration;
using System.Net;

namespace Worker.Common.Api
{
    public interface IWorkerApiClient
    {
        byte[] GetReport(string url);
    }

    public class WorkerApiClient : IWorkerApiClient
    {
        public byte[] GetReport(string parameters)
        {
            using (var client = new WebClient())
            {
                SetClientSettings(client);

                var result = client.DownloadData(new Uri(client.BaseAddress + "Home/ExecuteReport/?" + parameters));

                return result;
            }
        }

        //we currently think this is good enough.
        private static void SetClientSettings(WebClient client)
        {
            client.UseDefaultCredentials = true;
            client.BaseAddress = ConfigurationManager.AppSettings["ReportApiBaseUrl"];
        }
    }
}
