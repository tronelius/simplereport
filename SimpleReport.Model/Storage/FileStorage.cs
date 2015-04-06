﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Newtonsoft.Json;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Logging;

namespace SimpleReport.Model.Storage
{
    public class FileStorage :IStorage
    {
        private readonly ILogger _logger;
        private string _filename;
        private ReportDataModel _dataModel;

        //instanciate once every request, keep the model in memory during the request.
        public FileStorage(ILogger logger)
        {
            _logger = logger;
            _filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "datamodel.json");
            try {
                _dataModel = LoadModel();
            }
            catch (FileNotFoundException fex)
            {
                //autoinitialize storage
                logger.Warn("filestorage not initialized, file not found at:"+fex.FileName);
                InitializeStorage();
                _dataModel = LoadModel();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read Reports from file!", ex);
            }
        }

        public void InitializeStorage()
        {
            ReportDataModel model = new ReportDataModel();
            SaveModel(model);
        }

        public ReportDataModel LoadModel()
        {
            using (FileStream fs = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serializer.TypeNameHandling = TypeNameHandling.Objects;
                TextReader treader = new StreamReader(fs);
                JsonReader reader = new JsonTextReader(treader);
                ReportDataModel data = serializer.Deserialize<ReportDataModel>(reader);
                //todo handle empty file...
                return data;
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

        public IEnumerable<Report> GetAllReports()
        {
            var reports = _dataModel.Reports;
            reports.ForEach(LoadAndSetAccess);
            return reports;
        }

        public Report GetReport(Guid id)
        {
            var report =_dataModel.Reports.FirstOrDefault(r => r.Id == id);
            if (report == null)
                throw new EntityNotFoundException("Report not found");
            LoadAndSetConnection(report);
            LoadAndSetAccess(report);
            return report;
        }

        private void LoadAndSetConnection(LookupReport report)
        {
            var connection = GetConnection(report.ConnectionId);
            if (connection == null)
                throw new EntityNotFoundException("This report has a connection that cannot be found");
            report.Connection = connection;
        }

        private void LoadAndSetAccess(Report report)
        {
            var access = GetAccessList(report.AccessId);
            report.Access = access;
        }

        public bool SaveReport(Report report)
        {
            Report existingReport = _dataModel.Reports.FirstOrDefault(r => r.Id == report.Id);
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
            var report =  _dataModel.LookupReports.FirstOrDefault(c => c.Id == id);
            if (report == null)
                throw new EntityNotFoundException("Lookup report not found");
            LoadAndSetConnection(report);
            return report;
        }

        public bool SaveLookupReport(LookupReport lookupReport)
        {
            LookupReport existing = _dataModel.LookupReports.FirstOrDefault(r => r.Id == lookupReport.Id);
            if (existing != null)
                _dataModel.LookupReports.Remove(existing);
            _dataModel.LookupReports.Add(lookupReport);
            SaveModel(_dataModel);
            return true;          
        }


        public IEnumerable<Access> GetAccessLists()
        {
            return _dataModel.AccessLists;
        }

        public Access GetAccessList(Guid id)
        {
            var accesslist = _dataModel.AccessLists.FirstOrDefault(c => c.Id == id);
            return accesslist;
        }

        public bool SaveAccessList(Access accesslist)
        {
            Access existing = _dataModel.AccessLists.FirstOrDefault(r => r.Id == accesslist.Id);
            if (existing != null)
                _dataModel.AccessLists.Remove(existing);
            _dataModel.AccessLists.Add(accesslist);
            SaveModel(_dataModel);
            return true;      
        }

        public Settings GetSettings()
        {
            return _dataModel.Settings;
        }

        public bool SaveSettings(Settings settings)
        {
            _dataModel.Settings = settings;
            SaveModel(_dataModel);
            return true;
        }
    }
}
