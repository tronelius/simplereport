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
    /// <summary>
    /// Used to replace xml code and remove styling from rtf text
    /// </summary>
    public class ValueReplacer : IValueReplacer
    {
        public string Replace(string inputstring)
        {
            var value = IsRtf(inputstring) ? RtfToText(inputstring) : inputstring;
            return value;
        }
        private static bool IsRtf(string text)
        {
            return text.TrimStart().StartsWith("{\\rtf1", StringComparison.Ordinal);
        }
        private static string RtfToText(string text)
        {
            try
            {
                var rtBox = new System.Windows.Forms.RichTextBox {Rtf = text};
                text = rtBox.Text;
            } catch (Exception ex)
            {
               // do nothing, just return faulty rtf
            }

            return text;
        }
    }
}
