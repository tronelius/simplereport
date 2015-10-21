using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleReport.Model.Storage
{
   public class SQLStorage : BaseDapperRepo, IStorage
   {
       public SQLStorage(string connectionstring) : base(connectionstring){}

       public ReportDataModel LoadModel()
       {
           var reportDataModel = new ReportDataModel();
            //reportDataModel.AccessLists
           return reportDataModel;
       }

       public void SaveModel(ReportDataModel data)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<Report> GetAllReports()
       {
           return GetResults<Report>("select Id,Name,Group,Description,AccessId,ConnectionId,SQL,ResultType,MailSubject,MailText,ReportOwnerAccessId,OnScreenFormatAllowed,TemplateEditorAccessStyle,SubscriptionAccessStyle from Report",null);
       }

       public Report GetReport(Guid id)
       {
            return GetFirstResult<Report>("select Id,Name,Group,Description,AccessId,ConnectionId,SQL,ResultType,MailSubject,MailText,ReportOwnerAccessId,OnScreenFormatAllowed,TemplateEditorAccessStyle,SubscriptionAccessStyle from Report where id=@id", new {id});
        }

       public bool SaveReport(Report report)
       {
           if (Upsert(report))
           {
               return Upsert(report.Parameters);
           }
           return false;
       }

       public IEnumerable<Connection> GetConnections()
       {
           throw new NotImplementedException();
       }

       public Connection GetConnection(Guid id)
       {
           throw new NotImplementedException();
       }

       public bool SaveConnection(Connection connection)
       {
           return Upsert(connection);
       }

       public IEnumerable<LookupReport> GetLookupReports()
       {
           throw new NotImplementedException();
       }

       public LookupReport GetLookupReport(Guid id)
       {
           throw new NotImplementedException();
       }

       public bool SaveLookupReport(LookupReport lookupReport)
       {
           throw new NotImplementedException();
       }

       public IEnumerable<Access> GetAccessLists()
       {
           throw new NotImplementedException();
       }

       public Access GetAccessList(Guid? id)
       {
           throw new NotImplementedException();
       }

       public bool SaveAccessList(Access accesslist)
       {
           throw new NotImplementedException();
       }

       public DeleteInfo DeleteAccessList(Access acc)
       {
           throw new NotImplementedException();
       }

       public Settings GetSettings()
       {
           throw new NotImplementedException();
       }

       public bool SaveSettings(Settings settings)
       {
           throw new NotImplementedException();
       }

       public DeleteInfo DeleteConnection(Connection connection)
       {
           throw new NotImplementedException();
       }

       public DeleteInfo DeleteLookupReport(LookupReport lookupReport)
       {
           throw new NotImplementedException();
       }

       public DeleteInfo DeleteReport(Report report)
       {
           throw new NotImplementedException();
       }

       public void SaveTemplate(byte[] file, Guid reportId)
       {
           throw new NotImplementedException();
       }

       public Template GetTemplate(Guid reportId)
       {
           throw new NotImplementedException();
       }

       public void DeleteTemplate(Guid reportId)
       {
           throw new NotImplementedException();
       }
    }
}
