using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleReport.Model.Replacers;

namespace SimpleReportTests
{
    [TestFixture]
    public class ReplacerTests
    {
        [TestCase("@from",":from_replaced",Description = "reserved word full match 'from', replaced")]
        [TestCase("@FROM", ":from_replaced", Description = "reserved word full match uppercase 'FROM', replaced and lowercased")]
        [TestCase("@rom", ":rom", Description = "Non reserved word")]
        [TestCase("@from_to", ":from_to", Description = "parameter contains reserved word, no replace")]
        [TestCase(":from", ":from_replaced", Description = "reserved word full match 'from', but with another prefix, replaced")]
        public void OracleReplaceTests(string input, string expectedoutput)
        {
            IReplacer replacer = new OracleReservedWordsReplacer();
            var output = replacer.Replace(input);
            Assert.That(output, Is.EqualTo(expectedoutput));
        }
    }
}
