using System;
using System.Collections.Generic;
using System.Security.Principal;

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
        Access GetAccessList(Guid id);
        bool SaveAccessList(Access accesslist);
        Settings GetSettings();
        bool SaveSettings(Settings settings);
    }
}
