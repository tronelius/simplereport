using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Newtonsoft.Json;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Result;
using SimpleReport.Model.Storage.JsonConverters;
using SimpleReport.Model.Subscriptions;

namespace SimpleReport.Model.Storage
{
    public class FileStorage : IStorage
    {
        private readonly ILogger _logger;
        private readonly IStorageHelper _storageHelper;
        private string _filename;
        private ReportDataModel _dataModel;

        //instanciate once every request, keep the model in memory during the request.
        public FileStorage(ILogger logger, IStorageHelper storageHelper)
        {
            _logger = logger;
            _storageHelper = storageHelper;
            _filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "datamodel.json");
            try
            {
                _dataModel = LoadModel();
            }
            catch (FileNotFoundException fex)
            {
                //autoinitialize storage
                logger.Warn("filestorage not initialized, file not found at:" + fex.FileName);
                InitializeStorage();
                _dataModel = LoadModel();
            }
            catch (Exception ex)
            {
                logger.Error("Error deserializing file from disk" + ex);
                throw new Exception("Could not read Reports from file!", ex);
            }
        }

        public void InitializeStorage()
        {
            using (File.Create(_filename)) { 
                ReportDataModel model = new ReportDataModel();
                SaveModel(model);
            }
        }

        public ReportDataModel LoadModel()
        {
            using (FileStream fs = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return _storageHelper.ReadModelFromStream(fs);
            }   
        }

       

        public void SaveModel(ReportDataModel data)
        {
            using (FileStream fs = File.Open(_filename, FileMode.Truncate, FileAccess.Write)) {
                _storageHelper.WriteModelToStream(data, fs);
            }
        }

        public void ClearModel()
            {
            SaveModel(null);
        }

        public void SaveTemplate(byte[] data, string fileEnding, Guid reportId)
        {
            if (!fileEnding.StartsWith("."))
                fileEnding = "." + fileEnding;

            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/Templates", reportId + fileEnding);
            File.WriteAllBytes(filepath, data);
        }

        public Template GetTemplate(Guid reportId)
        {
            var filepath = GetFilePath(reportId);
            var bytes = File.ReadAllBytes(filepath);
            return new Template { Bytes = bytes, Filename = Path.GetFileName(filepath) };
        }

        public void DeleteTemplate(Guid reportId)
        {
            var filepath = GetFilePath(reportId);
            File.Delete(filepath);
        }


        private string GetFilePath(Guid reportId)
        {
            var id = reportId.ToString();
            return Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/Templates")).First(x => Path.GetFileNameWithoutExtension(x) == id);
        }

        public IEnumerable<ReportInfo> GetAllReportInfos()
        {
            var reports = _dataModel.Reports;
            reports.ForEach(LoadAndSetAccess);
            return reports;
        }

        public IEnumerable<Report> GetAllReports(bool includeLinkedReport = false)
        {
            var reports = _dataModel.Reports;
            reports.ForEach(LoadAndSetAccess);
            return reports;
        }

        public Report GetReport(Guid id)
        {
            var report = _dataModel.Reports.FirstOrDefault(r => r.Id == id);
            if (report == null)
                return null; //throw new EntityNotFoundException("Report not found");

            LoadAndSetConnection(report);
            LoadAndSetAccess(report);
            return report;
        }

        private void LoadAndSetConnection(LookupReport report)
        {
            if (report.ConnectionId != null)
            {
                var connection = GetConnection(report.ConnectionId.Value);
                if (connection == null)
                    throw new EntityNotFoundException("This report has a connection that cannot be found");
                report.Connection = connection;
            }
        }

        private void LoadAndSetAccess(ReportInfo report)
        {
            var access = GetAccessList(report.AccessId);
            report.Access = access;

            var taccess = GetAccessList(report.ReportOwnerAccessId);
            report.ReportOwnerAccess = taccess;
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

        public DeleteInfo DeleteReport(ReportInfo report)
        {
            Report existing = _dataModel.Reports.FirstOrDefault(r => r.Id == report.Id);
            if (existing == null)
                return new DeleteInfo(false, "Report doesn't exists");

            _dataModel.Reports.Remove(existing);
            SaveModel(_dataModel);

            if(existing.HasTemplate)
                DeleteTemplate(report.Id);

            return new DeleteInfo(true, "Report was deleted");
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

        public DeleteInfo DeleteConnection(Connection connection)
        {
            Connection existing = _dataModel.Connections.FirstOrDefault(r => r.Id == connection.Id);
            if (existing == null)
                return new DeleteInfo(false, "Connection doesn't exists");

            IEnumerable<Report> existingreports = _dataModel.Reports.Where(r => r.ConnectionId == connection.Id);
            if (existingreports.Any())
                return new DeleteInfo(false, "Connection cannot be deleted, it's used by other reports.", existingreports);

            _dataModel.Connections.Remove(existing);
            SaveModel(_dataModel);
            return new DeleteInfo(true, "Connection was deleted");
        }


        public IEnumerable<LookupReport> GetLookupReports()
        {
            return _dataModel.LookupReports;
        }

        public LookupReport GetLookupReport(Guid id)
        {
            var report = _dataModel.LookupReports.FirstOrDefault(c => c.Id == id);
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

        public DeleteInfo DeleteLookupReport(LookupReport lookupReport)
        {
            LookupReport existing = _dataModel.LookupReports.FirstOrDefault(r => r.Id == lookupReport.Id);
            if (existing == null)
                return new DeleteInfo(false, "Lookup report don't exists");

            IEnumerable<Report> existingreports = _dataModel.Reports.Where(r => r.Parameters.Any(p => p.LookupReportId == lookupReport.Id));
            if (existingreports.Any())
                return new DeleteInfo(false, "Lookup report cannot be deleted, it's used by other reports.", existingreports);

            _dataModel.LookupReports.Remove(existing);
            SaveModel(_dataModel);
            return new DeleteInfo(true, "Lookup report was deleted");
        }


        public IEnumerable<Access> GetAccessLists()
        {
            return _dataModel.AccessLists;
        }

        public Access GetAccessList(Guid? id)
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

        public DeleteInfo DeleteAccessList(Access acc)
        {
            var existing = _dataModel.AccessLists.FirstOrDefault(a => a.Id == acc.Id);
            if (existing == null)
                return new DeleteInfo(false, "Accesslist don't exists");

            IEnumerable<Report> existingreportsWithAccesslist = _dataModel.Reports.Where(r => r.AccessId == acc.Id);
            if (existingreportsWithAccesslist.Any())
                return new DeleteInfo(false, "Accesslist cannot be deleted, it's used by other reports.", existingreportsWithAccesslist);
            
            _dataModel.AccessLists.Remove(existing);
            SaveModel(_dataModel);
            return new DeleteInfo(true, "Accesslist was deleted");
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

        public TypeAheadReport GetTypeAheadReport(Guid typeAheadid)
        {
            throw new NotImplementedException();
        }

        public DeleteInfo DeleteTypeAheadReport(Guid typeAheadid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TypeAheadReport> GetTypeAheadReports()
        {
            throw new NotImplementedException();
        }

        public bool SaveTypeAheadReport(TypeAheadReport report)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Subscription> GetSubscriptions()
        {
            return _dataModel.Subscriptions;
        }

        public bool SaveSubscription(Subscription subscription)
        {
            Subscription existing = _dataModel.Subscriptions.FirstOrDefault(s => s.ReportId == subscription.ReportId);
            if (existing != null)
                _dataModel.Subscriptions.Remove(existing);
            _dataModel.Subscriptions.Add(subscription);
            SaveModel(_dataModel);
            return true;      
        }

        public IEnumerable<Schedule> GetSchedules()
        {
            return _dataModel.Schedules;
        }

        public bool SaveSchedule(Schedule schedule)
        {
            Schedule existing = _dataModel.Schedules.FirstOrDefault(s => s.Id == schedule.Id);
            if (existing != null)
                _dataModel.Schedules.Remove(existing);
            _dataModel.Schedules.Add(schedule);
            SaveModel(_dataModel);
            return true;      
        }
    }
}
