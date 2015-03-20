using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using SimpleReport.Model.Extensions;

namespace SimpleReport.Model
{
    
    public class ParameterList : List<Parameter>
    {
        public ParameterList(List<Parameter> parameters)
        {
            this.AddRange(parameters);
        }

        public ParameterList()
        {
        }

        public List<SqlParameter> CreateParameters()
        {
            List<SqlParameter> paramList = new List<SqlParameter>();
            foreach (Parameter parameter in this)
            {
                paramList.AddRange(parameter.GetSqlParameter());
            } 
            return paramList;
        } 

        public void ReadParameters(NameValueCollection querystring)
        {
            foreach (Parameter par in this)
            {
                //todo validate
                par.Value = querystring[par.Key];
            }
        }
    }


    
    public class Parameter
    {
        public const char SPLITCHAR = '_';

        public string Label { get; set; }
        [Required]
        public string SqlKey { get; set; }
        public string Value { get; set; }
        public ParameterInputType InputType { get; set; }
        public bool Mandatory { get; set; }
        public string HelpText { get; set; }
        public string Key { get { return SqlKey.Replace("@", ""); } }
        
        public Dictionary<string, string> Choices { get; protected set; }
        public Guid LookupReportId { get; set; }

        public Parameter()
        {
            Choices = new Dictionary<string, string>();
        }

        public Parameter(string label, string sqlKey, string value, ParameterInputType inputType, bool mandatory, string helptext) : this()
        {
            //TODO validate
            Label = label;
            SqlKey = sqlKey;
            Value = value;
            InputType = inputType;
            Mandatory = mandatory;
            HelpText = helptext;
        }

        public IEnumerable<SqlParameter> GetSqlParameterForPeriod()
        {
            string[] valueList = Value.Split(SPLITCHAR);
            string[] keyList = SqlKey.Split(SPLITCHAR);

            if (valueList.Length != keyList.Length)
                throw new Exception(string.Format("ValueList length {0} is different from KeyList length {1}", valueList.Length.ToString(), keyList.Length.ToString()));

            for (int i = 0; i < valueList.Length; i++)
            {
                yield return new SqlParameter(keyList[i], valueList[i]);
            }
        }

        public IEnumerable<SqlParameter> GetSqlParameter()
        {
            if (InputType == ParameterInputType.Period)
                return GetSqlParameterForPeriod();
            return new List<SqlParameter>() {new SqlParameter(this.SqlKey, this.Value)};
        }

        public bool IsValid()
        {
            if (Mandatory && string.IsNullOrWhiteSpace(Value))
                   return false;
            return true;
        }

        public void SetDefaultValuesForPeriod()
        {
            Choices = new Dictionary<string, string>();
            DateTime now = DateTime.Now;
            Choices.Add(now.GetFirstDateOfWeek().ToShortDateString() + SPLITCHAR + DateTime.Today.ToShortDateString(), "This Week");
            Choices.Add(now.GetFirstDateOfWeek().AddDays(-7).ToShortDateString() + SPLITCHAR + now.GetLastDateOfWeek().AddDays(-7).ToShortDateString(), "Last Week");
            Choices.Add(now.GetFirstDateOfMonth().ToShortDateString() + SPLITCHAR + now.GetLastDateOfMonth().ToShortDateString(), "This Month");
            Choices.Add(now.GetFirstDateOfMonth().AddMonths(-1).ToShortDateString() + SPLITCHAR + now.GetLastDateOfMonth().AddMonths(-1).ToShortDateString(), "Last Month");
            Choices.Add(now.GetFirstDayOfQuarter().ToShortDateString() + SPLITCHAR + now.GetLastDayOfQuarter().ToShortDateString(), "This Quarter");
            Choices.Add(now.GetFirstDayOfLastQuarter().ToShortDateString() + SPLITCHAR + now.GetLastDayOfLastQuarter().ToShortDateString(), "Last Quarter");
            Choices.Add(now.GetFirstDayOfYear().ToShortDateString() + SPLITCHAR + now.GetLastDayOfYear().ToShortDateString(), "This year");
            Choices.Add(now.GetFirstDayOfLastYear().ToShortDateString() + SPLITCHAR + now.GetLastDayOfLastYear().ToShortDateString(), "Last year");
        }
    }

  
    public enum ParameterInputType
    {
        String = 0,
        Integer = 1,
        Date=2,
        Period = 3,
        Lookup = 4
    }
}