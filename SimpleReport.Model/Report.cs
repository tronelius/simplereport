﻿using System;
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
      
        public ResultType ResultType { get; set; }
        public ParameterList Parameters { get; set; }
        public bool HasTemplate { get; set; }

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

        public Result ExecuteWithTemplate(byte[] templateData)
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");
            
            DataTable result = ADO.GetResults(Connection, Sql, Parameters.CreateParameters());
            return new Result(this.ResultType, result,this, templateData);
        }

        public bool IsAvailableForMe(IPrincipal user, Access adminAccess)
        {
            return (Access == null || Access.IsAvailableForMe(user) || adminAccess.IsAvailableForMe(user));
        }

        public void ReadParameters(NameValueCollection queryString)
        {
            Parameters.ReadParameters(queryString);
        }

        public bool IsAllowedToEditTemplate(IPrincipal user, Access adminAccess)
        {
            if (TemplateEditor == TemplateEditor.Anyone)
                return true;

            if (adminAccess.IsAvailableForMe(user))
                return true;

            if (TemplateEditor == TemplateEditor.ReportOwner)
                return TemplateAccess != null && TemplateAccess.IsAvailableForMe(user);

            return false;
        }
    }

}
