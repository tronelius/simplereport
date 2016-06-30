using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleReport.Model.DbExecutor;
using SimpleReport.Model.Extensions;

namespace SimpleReport.Model
{

    public class ParameterList : List<Parameter>
    {
        public ParameterList()
        {
        }

        public ParameterList(IEnumerable<Parameter> collection) : base(collection)
        {
        }

        public List<DbParameter> CreateParameters(string sql, Action<string> updateSqlAction, IDbExecutor db)
        {
            List<DbParameter> paramList = new List<DbParameter>();
            foreach (Parameter parameter in this)
            {
                paramList.AddRange(parameter.GetDbParameter(sql, updateSqlAction, db));
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

        public List<KeyValuePair<string, string>> Choices { get; protected set; }

        private Guid? LookupReportIdField=null;
        public Guid? LookupReportId //because this has been serialized as Guid.empty in the past.
        {
            get { return LookupReportIdField; }
            set
            {
                if (value == Guid.Empty)
                    LookupReportIdField = null;
                else
                    LookupReportIdField = value;
            }
        }

        public Guid? TypeAheadReportId { get; set; }

        [NonSerialized]
        public LookupReport LookupReport;
        [NonSerialized]
        public TypeAheadReport TypeAheadReport;

        public Guid ReportId { get; set; }
        public Parameter()
        {
            Choices = new List<KeyValuePair<string, string>>();
        }


        public IEnumerable<DbParameter> GetDbParameterForPeriod(IDbExecutor db)
        {
            string[] valueList = Value.Split(SPLITCHAR);
            string[] keyList = SqlKey.Split(SPLITCHAR);

            if (valueList.Length == 1)
            {
                ParameterPeriods period;
                if (Enum.TryParse(valueList[0], out period))
                {
                    valueList = GetValueListBasedOnPeriod(period);
                }
            }
            else if (valueList[0].Contains(":"))//this is custom, it starts with the enum value and then the actual value. ENUM:FROM_TO
            {
                valueList = Value.Split(':')[1].Split(SPLITCHAR);
            }

            if (valueList.Length != keyList.Length)
                throw new Exception(string.Format("ValueList length {0} is different from KeyList length {1}", valueList.Length.ToString(), keyList.Length.ToString()));

            for (int i = 0; i < valueList.Length; i++)
            {
                yield return db.CreateParameter(keyList[i], valueList[i]);
            }
        }

        public List<DbParameter> GetDbParameterForLookup(string query, Action<string> updateSqlAction, IDbExecutor db)
        {
            var dbParameters = new List<DbParameter>();
            string[] valueList = null;

            if (!string.IsNullOrWhiteSpace(Value))
                valueList = Value.Split(',');
            
            //We replace @param with @param1,@param2 to handle in-clauses
            if (valueList != null && valueList.Length > 1)
            {
                var newParams = new List<string>();
                for (int i = 0; i < valueList.Length; i++)
                {
                    var np = i!=0?"@" + Key + i: "@"+Key;
                    newParams.Add(np);
                    dbParameters.Add(db.CreateParameter(np, valueList[i]));
                }

                var replacement = string.Join(",", newParams);

                //Regular expression to find only those parameters that is surrounded by parentesis to match a "in (@paramname)" situation or a "in(@paramname)" situation.
                string findWhereIn = @"(?<=in\()@" + Key + @"(?=\))| (?<=in \()@" + Key + @"(?=\))";
                var newQuery = Regex.Replace(query, findWhereIn, replacement);
                updateSqlAction(newQuery);
            }
            else
            {
                dbParameters.Add(db.CreateParameter(Key, Value));
        }

            return dbParameters;
        }

        private string[] GetValueListBasedOnPeriod(ParameterPeriods period)
        {
            DateTime now = DateTime.Now;
            switch (period)
            {
                case ParameterPeriods.ThisWeek:
                    return new[] { now.GetFirstDateOfWeek().ToShortDateString(), DateTime.Today.ToShortDateString() };
                case ParameterPeriods.LastWeek:
                    return new[] { now.GetFirstDateOfWeek().AddDays(-7).ToShortDateString(), now.GetLastDateOfWeek().AddDays(-7).ToShortDateString() };
                case ParameterPeriods.ThisMonth:
                    return new[] { now.GetFirstDateOfMonth().ToShortDateString(), now.GetLastDateOfMonth().ToShortDateString() };
                case ParameterPeriods.LastMonth:
                    return new[] { now.GetFirstDateOfMonth().AddMonths(-1).ToShortDateString(), now.GetLastDateOfMonth().AddMonths(-1).ToShortDateString() };
                case ParameterPeriods.ThisQuarter:
                    return new[] { now.GetFirstDayOfQuarter().ToShortDateString(), now.GetLastDayOfQuarter().ToShortDateString() };
                case ParameterPeriods.LastQuarter:
                    return new[] { now.GetFirstDayOfLastQuarter().ToShortDateString(), now.GetLastDayOfLastQuarter().ToShortDateString() };
                case ParameterPeriods.ThisYear:
                    return new[] { now.GetFirstDayOfYear().ToShortDateString(), now.GetLastDayOfYear().ToShortDateString() };
                case ParameterPeriods.LastYear:
                    return new[] { now.GetFirstDayOfLastYear().ToShortDateString(), now.GetLastDayOfLastYear().ToShortDateString() };
                default:
                    throw new NotSupportedException("Enum value not supported:" + period);
            }
        }

        public IEnumerable<DbParameter> GetDbParameter(string query, Action<string> updateSqlAction, IDbExecutor db)
        {
            if (InputType == ParameterInputType.Period)
                return GetDbParameterForPeriod(db);
            if (InputType == ParameterInputType.Lookup || InputType == ParameterInputType.LookupMultipleChoice)
                return GetDbParameterForLookup(query, updateSqlAction, db);
            return new List<DbParameter>() { db.CreateParameter(this.SqlKey, this.Value) };
        }

        public bool IsValid()
        {
            if (Mandatory && string.IsNullOrWhiteSpace(Value))
                return false;
            return true;
        }

        public void SetDefaultValuesForPeriod()
        {
            var dict = new Dictionary<string, string>();
            dict.Add(((int)ParameterPeriods.ThisWeek).ToString(), "This Week");
            dict.Add(((int)ParameterPeriods.LastWeek).ToString(), "Last Week");
            dict.Add(((int)ParameterPeriods.ThisMonth).ToString(), "This Month");
            dict.Add(((int)ParameterPeriods.LastMonth).ToString(), "Last Month");
            dict.Add(((int)ParameterPeriods.ThisQuarter).ToString(), "This Quarter");
            dict.Add(((int)ParameterPeriods.LastQuarter).ToString(), "Last Quarter");
            dict.Add(((int)ParameterPeriods.ThisYear).ToString(), "This Year");
            dict.Add(((int)ParameterPeriods.LastYear).ToString(), "Last Year");
            dict.Add(((int)ParameterPeriods.Custom).ToString(), "Custom");

            Choices = dict.ToList();
        }
    }

    public enum ParameterPeriods
    {
        ThisWeek = 1,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisQuarter,
        LastQuarter,
        ThisYear,
        LastYear,
        Custom = 9999
    }


    public enum ParameterInputType
    {
        String = 0,
        Integer = 1,
        Date = 2,
        Period = 3,
        Lookup = 4,
        LookupMultipleChoice = 5,
        SyncedDate = 6,
        SyncedRunningDate = 7,
        TypeAhead=8
    }
}