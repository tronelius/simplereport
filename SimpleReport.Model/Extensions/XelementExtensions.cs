using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenXmlPowerTools;

namespace SimpleReport.Model.Extensions
{
    public static class XElementExtensions
    {
        public static string SdtTagName(this XElement sdt)
        {
            if (sdt.Name != W.sdt) return null;

            try
            {
                return sdt
                    .Element(W.sdtPr)
                    .Element(W.tag)
                    .Attribute(W.val)
                    .Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
