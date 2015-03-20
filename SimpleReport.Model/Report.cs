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
        public ReportInfo(string id, string name, string description)
        {
            Guid guid;
            if (!Guid.TryParse(id, out guid))
                throw new ArgumentException("Supplied string ID is not a valid Unique Identifier");

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception(string.Format("Missing name in report"));

            ID = guid;
            Name = name;
            Description = description;
            //Errors = new List<ErrorInfo>();
        }

    }
    
    public class LookupReport : ReportInfo
    {
        protected Connection Connection;
        [Required]
        public string ConnectionStringName { get; set; }
        [Required]
        public string Sql { get; set; }

        public LookupReport()
        {
        }

        public LookupReport(string id, string name, string description, string connectionStringName,string sql) : base(id, name, description)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new Exception(string.Format("Missing SQL in report with Name:{0}", name));

            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new Exception(string.Format("Missing Connectionstring Name in report with Name:{0}", name));

            ConnectionStringName = connectionStringName;
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

        public void SetConnection(Connection conn)
        {
            //Think, anything more?
            Connection = conn;
        }
    }

    
    public class Report : LookupReport
    {
        public ResultType ResultType { get; set; }
        public ParameterList Parameters { get; set; }

        public Report()
        {
            ResultType= ResultType.SimpleExcel;
            Parameters = new ParameterList();
        } 
        /*: this(Guid.NewGuid().ToString(),string.Empty, string.Empty,string.Empty,string.Empty,new List<Parameter>(),ResultType.SimpleExcel )
        {}*/

        public Report(string id, string name, string description, string connectionStringName, string sql, List<Parameter> parameters, ResultType resultType) : base(id,name, description,connectionStringName,sql)
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
