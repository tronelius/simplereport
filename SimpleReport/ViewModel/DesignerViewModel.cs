using Newtonsoft.Json;
using SimpleReport.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleReport.Model.Extensions;

namespace SimpleReport.ViewModel
{
    public class DesignerViewModel 
    {
        public Report Report { get; set; }
        public List<Report> Reports { get; set; }
        public IEnumerable<KeyValue> InputTypes {get;set;}

        public DesignerViewModel()
        {
            ParameterInputType types = new ParameterInputType();
            InputTypes = types.ToKeyValues();
        }
    }
}