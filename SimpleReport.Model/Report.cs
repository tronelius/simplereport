using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace SimpleReport.Model
{
    public class ReportInfo : ValidatableEntity
    {
        public Guid ID { get; set; }
        [Required][StringLength(300)] 
        public string Name { get; set; }
        [StringLength(50)]
        public string Group { get; set; }
        [StringLength(1000)] 
        public string Description { get; set; }
        
        /*public List<ErrorInfo> Errors { get; private set; } 
        
        public bool CanExecute()
        {
            return Errors.Count(e => e.Level == ErrorLevel.Fatal)> 0;
        }

        public bool IsValid()
        {
            return Errors.Count(e => e.Level > ErrorLevel.Warning) > 0;
        }*/

        public ReportInfo(){}
        public ReportInfo(Guid id, string name, string description, string group)
        {
            //Guid guid;
            //if (!Guid.TryParse(id, out guid))
            //    throw new ArgumentException("Supplied string ID is not a valid Unique Identifier");

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception(string.Format("Missing name in report"));

            ID = id;
            Name = name;
            Description = description;
            Group = @group;
            //Errors = new List<ErrorInfo>();
        }

    }
    
    public class LookupReport : ReportInfo
    {
        [NonSerialized]
        public Connection Connection;
        [Required]
        public Guid ConnectionId { get; set; }
        [Required]
        public string Sql { get; set; }

        public LookupReport()
        {
        }

        public LookupReport(Guid id, string name, string description, Guid connectionId,string sql, string group) : base(id, name, description, group)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new Exception(string.Format("Missing SQL in report with Name:{0}", name));
            ConnectionId = connectionId;
            Sql = sql;
        }

        public Dictionary<string, string> Execute()
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");

            DataTable result = ADO.GetResults<dynamic>(Connection, Sql,null);
            Dictionary<string, string> collection = new Dictionary<string, string>();
            foreach (DataRow row in result.Rows)
            {
                collection.Add(row[0].ToString(),row[1].ToString());
            }
            return collection;
        }
    }

    public class Report : LookupReport
    {
        public Guid AccessId { get; set; }
        [NonSerialized] 
        public Access Access;
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

        public void ReadParameters(NameValueCollection queryString)
        {
            Parameters.ReadParameters(queryString);
        }
    }

}
