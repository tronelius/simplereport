using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SimpleReport.Model.Replacers
{

    public class XmlReplacer : IXmlReplacer
    {
        public string Replace(string inputstring)
        {
            return XmlTextEncoder.Encode(inputstring);
        }
    }
}
