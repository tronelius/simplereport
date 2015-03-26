using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SimpleReport.Model.Exceptions;

namespace SimpleReport.Model.Storage
{
    public class FileStorage :IStorage
    {
        private string _filename;
        private ReportDataModel _dataModel;

        //instanciate once every request, keep the model in memory during the request.
        public FileStorage()
        {
            _filename = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["datastorefilename"];
            _dataModel = LoadModel();
        }
        
        public ReportDataModel LoadModel()
        {
            using (FileStream fs = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serializer.TypeNameHandling = TypeNameHandling.Objects;

                    TextReader treader = new StreamReader(fs);
                    JsonReader reader = new JsonTextReader(treader);
                    ReportDataModel data = serializer.Deserialize<ReportDataModel>(reader);
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not read Reports from file!", ex);
                }
            }
        }

        public void SaveModel(ReportDataModel data)
        {
            using (FileStream fs = File.Open(_filename, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, data);
            }
        }

        public IEnumerable<Report> GetReports()
        {
            return _dataModel.Reports;
        }

        public Report GetReport(Guid id)
        {
            var report =_dataModel.Reports.FirstOrDefault(r => r.ID == id);
            if (report == null)
                throw new EntityNotFoundException("Report not found");
            LoadAndSetConnection(report);
            return report;
        }

        private void LoadAndSetConnection(LookupReport report)
        {
            var connection = GetConnection(report.ConnectionId);
            if (connection == null)
                throw new EntityNotFoundException("Report has a ConnectionId that cannot be found");
            report.Connection = connection;
        }

        public bool SaveReport(Report report)
        {
            Report existingReport = _dataModel.Reports.FirstOrDefault(r => r.ID == report.ID);
            if (existingReport != null)
                _dataModel.Reports.Remove(existingReport);
            _dataModel.Reports.Add(report);
            SaveModel(_dataModel);
            return true;
        }

        public IEnumerable<Connection> GetConnections()
        {
            return _dataModel.Connections;
        }

        public Connection GetConnection(Guid id)
        {
            return _dataModel.Connections.FirstOrDefault(c => c.Id == id);
        }

        public bool SaveConnection(Connection connection)
        {
            Connection existing = _dataModel.Connections.FirstOrDefault(r => r.Id == connection.Id);
            if (existing != null)
                _dataModel.Connections.Remove(existing);
            _dataModel.Connections.Add(connection);
            SaveModel(_dataModel);
            return true;
        }

        public IEnumerable<LookupReport> GetLookupReports()
        {
            return _dataModel.LookupReports;
        }

        public LookupReport GetLookupReport(Guid id)
        {
            var report =  _dataModel.LookupReports.FirstOrDefault(c => c.ID == id);
            if (report == null)
                throw new EntityNotFoundException("Lookup report not found");
            LoadAndSetConnection(report);
            return report;
        }

        public bool SaveLookupReport(LookupReport lookupReport)
        {
            LookupReport existing = _dataModel.LookupReports.FirstOrDefault(r => r.ID == lookupReport.ID);
            if (existing != null)
                _dataModel.LookupReports.Remove(existing);
            _dataModel.LookupReports.Add(lookupReport);
            SaveModel(_dataModel);
            return true;          
        }

    }
}
