using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleReport.Model
{
    public interface IStorage
    {
        ReportManagerData LoadModel();
        void SaveModel(ReportManagerData data);
    }
}
