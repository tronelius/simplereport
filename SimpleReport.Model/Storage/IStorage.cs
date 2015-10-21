using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;

namespace SimpleReport.Model.Storage
{
    public interface IStorage
    {
        ReportDataModel LoadModel();
        void SaveModel(ReportDataModel data);

        IEnumerable<Report> GetAllReports();
        //IEnumerable<Report> GetReportsForMe(IPrincipal user);
        Report GetReport(Guid id);
        bool SaveReport(Report report);

        IEnumerable<Connection> GetConnections();
        Connection GetConnection(Guid id);
        bool SaveConnection(Connection connection);

        IEnumerable<LookupReport> GetLookupReports();
        LookupReport GetLookupReport(Guid id);
        bool SaveLookupReport(LookupReport lookupReport);

        IEnumerable<Access> GetAccessLists();
        Access GetAccessList(Guid? id);
        bool SaveAccessList(Access accesslist);
        DeleteInfo DeleteAccessList(Access acc);

        Settings GetSettings();
        bool SaveSettings(Settings settings);

        DeleteInfo DeleteConnection(Connection connection);
        DeleteInfo DeleteLookupReport(LookupReport lookupReport);
        DeleteInfo DeleteReport(Report report);
        void SaveTemplate(byte[] file, Guid reportId);
        Template GetTemplate(Guid reportId);
        void DeleteTemplate(Guid reportId);
    }
}
