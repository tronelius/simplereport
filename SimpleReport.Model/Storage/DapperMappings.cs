using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions.Mapper;

namespace SimpleReport.Model.Storage
{
    public class ReportMap : ClassMapper<Report>
    {
        public ReportMap()
        {
            Map(a => a.Parameters).Ignore();
            Map(a => a.HasTemplate).Ignore();
            AutoMap();
        }
    }
}
