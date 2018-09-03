using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using DapperExtensions;
using SimpleReport.Model.Constants;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Result;

namespace SimpleReport.Model.Storage.SQL
{
    public class SQLStorage : BaseDapperRepo, IStorage
    {
        private readonly IStorageHelper _storageHelper;

        public SQLStorage(IStorageHelper storageHelper) : base()
        {
            _storageHelper = storageHelper;
        }

        public SQLStorage(string connectionstring, IStorageHelper storageHelper) : base(connectionstring)
        {
            _storageHelper = storageHelper;
        }

        public ReportDataModel LoadModel()
        {
            var reportDataModel = new ReportDataModel();
            reportDataModel.AccessLists = GetAccessLists().ToList();
            reportDataModel.Connections = GetConnections().ToList();
            reportDataModel.LookupReports = GetLookupReports().ToList();
            reportDataModel.TypeAheadReports = GetTypeAheadReports().ToList();
            reportDataModel.Settings = GetSettings();
            reportDataModel.Reports = GetAllReports().ToList();
            return reportDataModel;
        }



        public void SaveModel(ReportDataModel data)
        {
            SaveSettings(data.Settings);
            data.AccessLists.ForEach(a => SaveAccessList(a));
            data.Connections.ForEach(c => SaveConnection(c));
            data.LookupReports.ForEach(l => SaveLookupReport(l));
            data.TypeAheadReports.ForEach(l => SaveTypeAheadReport(l));
            data.Reports.ForEach(r => SaveReport(r));
        }

        public void ClearModel()
        {
            ExecuteInTransaction((con, transaction) =>
            {
                con.Execute("Delete from Parameter", null, transaction);
                con.Execute("Delete from Report", null, transaction);
                con.Execute("Delete from LookupReport", null, transaction);
                con.Execute("Delete from TypeAheadReport", null, transaction);
                con.Execute("Delete from Connection", null, transaction);
                con.Execute("Delete from Access", null, transaction);
               //con.Execute("Delete from Settings", transaction); //don't delete settings, will mess up accessrights to admin.
           });

        }

        public IEnumerable<Report> GetAllReports(bool includeLinkedReport = false)
        {
            using (var cn = EnsureOpenConnection())
            {
                var reports = cn.Query<Report, Access, Access, Report>
                    ("select r.*, owner.*,acs.* from Report r " +
                    "left join access as owner on r.reportowneraccessid = owner.id " +
                    "left join access as acs on r.accessid = acs.id ", (rpt, owner, access) =>
                    {
                        rpt.ReportOwnerAccess = owner;
                        rpt.Access = access;
                        return rpt;
                    });
                var parameters = cn.Query<Parameter, LookupReport, Connection, TypeAheadReport, Connection, Parameter>("select p.*,look.*, conn.*, ahead.*, conn2.* from parameter p left join lookupReport look on p.lookupreportid = look.id left join connection conn on look.connectionid = conn.id" +
                                                                                                                       " left join TypeAheadReport ahead on p.typeaheadreportid = ahead.id left join connection conn2 on ahead.connectionid = conn2.id", (Parameter, LookupReport, Connection, TypeAheadReport, Connection2) =>
                {
                    Parameter.LookupReport = LookupReport;
                    if (LookupReport != null)
                        Parameter.LookupReport.Connection = Connection;

                    Parameter.TypeAheadReport = TypeAheadReport;
                    if (TypeAheadReport != null)
                        Parameter.TypeAheadReport.Connection = Connection2;

                    return Parameter;
                }).ToLookup(p => p.ReportId);
                foreach (var report in reports)
                {
                    if (parameters.Contains(report.Id))
                    {
                        report.Parameters = new ParameterList(parameters[report.Id]);
                    }

                    if (includeLinkedReport && report.ReportType == ReportType.MultiReport)
                    {
                        report.ReportList = GetAllLinkedReports(report.Id);
                    }
                }
                return reports;
            }
        }

        public IEnumerable<LinkedReport> GetAllLinkedReports(Guid id)
        {
            try
            {
                using (var cn = EnsureOpenConnection())
                {
                    var linkedReports = cn.Query<LinkedReport, Report, LinkedReport>
                    ("select linkedReport.*, report.Name as name from linkedReport linkedReport " +
                     "left join report as report on linkedReport.LinkedReportId = report.Id " +
                     "where linkedReport.ReportId = @id", (lr, report) =>
                        {
                            lr.Name = report.Name;
                            return lr;
                        }, new { id }, splitOn: "name");

                    return linkedReports;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                return null;
            }

        }
        public IEnumerable<ReportInfo> GetAllReportInfos()
        {
            using (var cn = EnsureOpenConnection())
            {
                return cn.Query<ReportInfo, Access, Access, ReportInfo>
                     ("select r.id, r.name, r.[group] ,r.description,r.accessid,r.ReportOwnerAccessId, owner.*,acs.* from Report r " +
                     "left join access as owner on r.reportowneraccessid = owner.id " +
                     "left join access as acs on r.accessid = acs.id ", (rpt, owner, access) =>
                       {
                          rpt.ReportOwnerAccess = owner;
                          rpt.Access = access;
                          return rpt;
                      });
            }
        }

        public Report GetReport(Guid id)
        {
            using (var cn = EnsureOpenConnection())
            {
                var report = cn.Query<Report, Connection, Access, Access, Report>("select r.*,conn.*,owner.*,acs.* from report r " +
                                                                      "inner join connection conn on r.connectionid = conn.id " +
                                                                      "left join access as owner on r.reportowneraccessid = owner.id " +
                                                                      "left join access as acs on r.accessid = acs.id " +
                                                                      "where r.id = @id", (rpt, conn, owner, access) =>
                                                                      {
                                                                          rpt.Connection = conn;
                                                                          rpt.ReportOwnerAccess = owner;
                                                                          rpt.Access = access;
                                                                          return rpt;
                                                                      }, new { id }).FirstOrDefault();
                if (report == null)
                    return null;

                var parameters = cn.Query<Parameter, LookupReport, Connection, TypeAheadReport, Connection, Parameter>("select p.*,look.*, conn.*, ahead.*, conn2.* from parameter p left join lookupReport look on p.lookupreportid = look.id left join connection conn on look.connectionid = conn.id " +
                                                                                          " left join TypeAheadReport ahead on p.typeaheadreportid = ahead.id left join connection conn2 on ahead.connectionid = conn2.id" +
                                                                                          " where reportid = @id", (Parameter, LookupReport, Connection, TypeAheadReport, Connection2) =>
                {
                    Parameter.LookupReport = LookupReport;
                    if (LookupReport != null)
                        Parameter.LookupReport.Connection = Connection;

                    Parameter.TypeAheadReport = TypeAheadReport;
                    if (TypeAheadReport != null)
                        Parameter.TypeAheadReport.Connection = Connection2;

                    return Parameter;
                }, new { id });
                report.Parameters = new ParameterList(parameters);
                if (report.ReportType == ReportType.MultiReport)
                {
                    report.ReportList = GetAllLinkedReports(id);
                }
                return report;
            }
        }

        public bool SaveReport(Report report)
        {
            if (Upsert(report))
            {
                Execute("delete from parameter where reportId = @reportid", new { reportid = report.Id });
                Execute("delete from linkedreport where reportId = @reportid", new { reportid = report.Id });

                using (var conn = EnsureOpenConnection())
                {
                    foreach (Parameter parameter in report.Parameters)
                    {
                        parameter.ReportId = report.Id;
                        conn.Insert(parameter);
                    }
                    if (report.ReportType == ReportType.MultiReport)
                    {
                        try
                        {
                            foreach (var listItem in report.ReportList)
                            {
                                listItem.ReportId = report.Id; // In case Report just was created
                                conn.Insert(listItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = ex.Message;
                        }
                    }
                  

                }
                return true;
            }
            return false;
        }

        public IEnumerable<Connection> GetConnections()
        {
            return GetResults<Connection>("Select * from Connection");
        }

        public Connection GetConnection(Guid id)
        {
            return GetFirstResult<Connection>("select * from Connection where id=@id", new { id });
        }

        public bool SaveConnection(Connection connection)
        {
            return Upsert(connection);
        }

        public IEnumerable<LookupReport> GetLookupReports()
        {
            return GetResults<LookupReport>("Select * from Lookupreport");
        }

        public LookupReport GetLookupReport(Guid id)
        {
            using (var conn = EnsureOpenConnection())
            {
                return conn.Query<LookupReport, Connection, LookupReport>("Select look.*,conn.* from lookupReport look left join connection conn on look.connectionid = conn.id where Id = @id",
                    (look, connection) =>
                    {
                        look.Connection = connection;
                        return look;
                    },
                    new { id }).FirstOrDefault();
            }
        }

        public bool SaveLookupReport(LookupReport lookupReport)
        {
            return Upsert(lookupReport);
        }

        public IEnumerable<Access> GetAccessLists()
        {
            return GetResults<Access>("Select * from access");
        }

        public Access GetAccessList(Guid? id)
        {
            return GetFirstResult<Access>("Select * from access where Id = @id", new { id });
        }

        public bool SaveAccessList(Access accesslist)
        {
            return Upsert(accesslist);
        }

        public DeleteInfo DeleteAccessList(Access acc)
        {
            IEnumerable<Report> existingreportsWithAccesslist = GetResults<Report>("Select * from report where reportOwnerAccessId=@accessid or AccessID=@accessid", new { accessid = acc.Id });
            if (existingreportsWithAccesslist.Any())
                return new DeleteInfo(false, "Accesslist cannot be deleted, it's used by other reports.", existingreportsWithAccesslist);

            var rowsAffected = Execute("delete from access where id = @accid", new { accid = acc.Id });
            if (rowsAffected == 0)
                return new DeleteInfo(false, "Accesslist don't exists");
            return new DeleteInfo(true, "Accesslist was deleted");
        }

        public Settings GetSettings()
        {
            var result = GetResults<SQLSetting>("select * from settings").ToDictionary(a => a.Name, a => a.Value);
            return Settings.CreateSettingsFromDictionary(result);
        }

        public bool SaveSettings(Settings settings)
        {
            using (var conn = EnsureOpenConnection())
            {
                conn.Execute("Delete from Settings");
                var dict = settings.ToDictionary();
                var values = dict.Select(a => new { Name = a.Key, Value = a.Value });
                return conn.Execute("insert into Settings(name, value) values(@Name, @Value)", values) == values.Count();
            }
        }

        public DeleteInfo DeleteConnection(Connection connection)
        {
            IEnumerable<Report> existingreports = GetResults<Report>("Select * from report where connectionID=@connectionid", new { connectionid = connection.Id });
            if (existingreports.Any())
                return new DeleteInfo(false, "Connection cannot be deleted, it's used by other reports.", existingreports);

            var rowsAffected = Execute("Delete from connection where Id=@connectionid", new { connectionid = connection.Id });
            if (rowsAffected == 0)
                return new DeleteInfo(false, "Connection doesn't exists");
            return new DeleteInfo(true, "Connection was deleted");
        }

        public DeleteInfo DeleteLookupReport(LookupReport lookupReport)
        {
            IEnumerable<Report> existingreports = GetResults<Report>("Select * from report where Id in (select reportID from parameter where lookupreportid = @lookupReportID)", new { lookupReportId = lookupReport.Id });
            if (existingreports.Any())
                return new DeleteInfo(false, "Lookup report cannot be deleted, it's used by other reports.", existingreports);

            var rowsAffected = Execute("Delete from lookupreport where Id=@reportID", new { reportID = lookupReport.Id });
            if (rowsAffected == 0)
                return new DeleteInfo(false, "Lookup report don't exists");
            return new DeleteInfo(true, "Lookup report was deleted");
        }

        public DeleteInfo DeleteReport(ReportInfo report)
        {
            int rowsAffected = 0;
            ExecuteInTransaction((con, transaction) =>
            {
                con.Execute("Delete from Parameter where reportid=@reportID", new { reportID = report.Id }, transaction);
                rowsAffected = con.Execute("Delete from report where Id=@reportID", new { reportID = report.Id }, transaction);

            });
            if (rowsAffected == 0)
                return new DeleteInfo(false, "Report doesn't exists");
            return new DeleteInfo(true, "Report was deleted");
        }

        public void SaveTemplate(byte[] file, string mimetype, Guid reportId)
        {
            Execute("Update Report set Template=@file, hasTemplate=1 where Id=@reportId", new { reportId, file });
        }

        public Template GetTemplate(Guid reportId)
        {
            Template template = GetFirstResult<Template>("select Template as bytes, TemplateFormat from Report where id=@reportId", new { reportId });
            template.Filename = "Template_" + reportId.ToString();
            if (template.TemplateFormat == TemplateFormat.Word)
                template.Filename += ".docx";
            else
                template.Filename += ".xlsx";

            return template;
        }

        public void DeleteTemplate(Guid reportId)
        {
            Execute("Update Report set Template=null, hasTemplate=0, TemplateFormat=0 where Id=@reportId", new { reportId });
        }

        public TypeAheadReport GetTypeAheadReport(Guid typeAheadid)
        {
            using (var conn = EnsureOpenConnection())
            {
                return conn.Query<TypeAheadReport, Connection, TypeAheadReport>
                ("select t.*, c.* from TypeAheadReport t inner join Connection c on t.connectionId = c.id where t.Id=@id", (typeahead, connection) =>
                {
                    typeahead.Connection = connection;
                    return typeahead;
                }, new { Id = typeAheadid }).FirstOrDefault();
            }
        }

        public DeleteInfo DeleteTypeAheadReport(Guid typeAheadid)
        {
            IEnumerable<Report> existingreports = GetResults<Report>("Select * from report where Id in (select reportID from parameter where typeaheadreportid = @typeaheadReportID)", new { typeaheadReportID = typeAheadid });
            if (existingreports.Any())
                return new DeleteInfo(false, "Typeahead report cannot be deleted, it's used by other reports.", existingreports);

            var rowsAffected = Execute("delete from TypeAheadReport where Id=@id", new { Id = typeAheadid });
            if (rowsAffected == 0)
                return new DeleteInfo(false, "Typeahead doesn't exists");
            return new DeleteInfo(true, "Typeahead was deleted");
        }

        public IEnumerable<TypeAheadReport> GetTypeAheadReports()
        {
            return GetResults<TypeAheadReport>("select * from TypeAheadReport ");
        }

        public bool SaveTypeAheadReport(TypeAheadReport report)
        {
            return Upsert(report);
        }
    }
}
