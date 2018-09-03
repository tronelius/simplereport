using System.Management.Instrumentation;
using DapperExtensions.Mapper;

namespace SimpleReport.Model.Storage.SQL
{
    public class ReportMap : ClassMapper<Report>
    {
        public ReportMap()
        {
            Map(a => a.Id).Key(KeyType.Assigned);
            Map(a => a.Parameters).Ignore();
            Map(a => a.ReportList).Ignore();
            AutoMap();
        }
    }

    public class ConnectionMap : ClassMapper<Connection>
    {
        public ConnectionMap()
        {
            Map(a => a.Id).Key(KeyType.Assigned);
            AutoMap();
        }
    }

    public class ParameterMap : ClassMapper<Parameter>
    {
        public ParameterMap()
        {
            Map(a => a.SqlKey).Key(KeyType.Assigned);
            Map(a => a.ReportId).Key(KeyType.Assigned);
            Map(a => a.Choices).Ignore();
            Map(a => a.Key).Ignore();
            AutoMap();
        }
    }

    public class LinkedReportMap : ClassMapper<LinkedReport>
    {
        public LinkedReportMap()
        {
            Map(a => a.ReportId).Key(KeyType.Assigned);
            Map(a => a.LinkedReportId).Key(KeyType.Assigned);
            Map(a => a.Name).Ignore();
            AutoMap();
        }
    }
    public class LookupReportMap : ClassMapper<LookupReport>
    {
        public LookupReportMap()
        {
            Map(a => a.Id).Key(KeyType.Assigned);
            Map(a => a.Group).Ignore();
            Map(a => a.Description).Ignore();
            Map(a => a.AccessId).Ignore();
            Map(a => a.ReportOwnerAccessId).Ignore();
            AutoMap();
        }
    }


    public class TypeAheadReportMap : ClassMapper<TypeAheadReport>
    {
        public TypeAheadReportMap()
        {
            Map(a => a.Id).Key(KeyType.Assigned);
            Map(a => a.Group).Ignore();
            Map(a => a.Description).Ignore();
            Map(a => a.AccessId).Ignore();
            Map(a => a.ReportOwnerAccessId).Ignore();
            AutoMap();
        }
    }

    public class AccessMap : ClassMapper<Access>
    {
        public AccessMap()
        {
            Map(a => a.Id).Key(KeyType.Assigned);
            Map(a => a.SplittedAdGroups).Ignore();
            AutoMap();
        }
    }
}
