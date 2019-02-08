using System;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using SimpleReport.Model.Subscriptions;

namespace SimpleReport.SubscriptionService
{
    public interface IWorkerApiClient
    {
        ReportResult GetReport(string url);
    }

    public class WorkerApiClient : IWorkerApiClient
    {
        public ReportResult GetReport(string parameters)
        {
            using (var client = new WebClient())
            {
                SetClientSettings(client);
                var result = client.DownloadData(new Uri(client.BaseAddress + "Home/ExecuteReport/?" + parameters));
                if (result.Length == 0) //only look for file if we have any result...
                    return null;
                
                var headerString = FixHeaderString(client.ResponseHeaders.Get("Content-Disposition"));
                var headers = ContentDispositionHeaderValue.Parse(headerString);
                return new ReportResult(result, headers.FileName.Replace("\"", ""));
                
            }
        }

        // Quickfix to fix header, for some reason it seems that the name gets wrong chars in it, and decoding it and decoding it back seems to fix it.
        private string FixHeaderString(string headerString)
        {
            headerString = HttpUtility.UrlDecode(headerString);
            headerString = HttpUtility.UrlEncode(headerString);
            return headerString;
        }

        //we currently think this is good enough.
        private static void SetClientSettings(WebClient client)
        {
            client.UseDefaultCredentials = true;
            client.BaseAddress = ConfigurationManager.AppSettings["ReportApiBaseUrl"];
        }
    }
}
