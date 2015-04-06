using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace SimpleReport.Model
{
    public class Report : LookupReport
    {
        //private string 
        public ResultType ResultType { get; set; }
        public ParameterList Parameters { get; set; }

        public Report()
        {
            ResultType= ResultType.SimpleExcel;
            Parameters = new ParameterList();
        } 

        public Report(Guid id, string name, string description, Guid connectionId, string sql, List<Parameter> parameters, ResultType resultType, string group) : base(id,name, description,connectionId,sql, group)
        {
            Parameters = new ParameterList(parameters);
            ResultType = resultType;
        }

        public bool IsParameterValueValid()
        {
            return Parameters.All(p => p.IsValid());
        }

        public new Result Execute()
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");
            
            DataTable result = ADO.GetResults<dynamic>(Connection, Sql, Parameters.CreateParameters());
            return new Result(this.ResultType, result,this);
        }

        public bool IsAvailableForMe(IPrincipal user, string adminAccessRole)
        {
            return (Access == null || user.IsInRole(Access.ADGroup) || (user.IsInRole(adminAccessRole) || adminAccessRole == null));
        }

        public void ReadParameters(NameValueCollection queryString)
        {
            Parameters.ReadParameters(queryString);
        }
    }

}
