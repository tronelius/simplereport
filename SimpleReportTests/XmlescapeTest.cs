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
    public class XmlescapeTest
    {
        [TestCase("", "", Description = "empty should return empty")]
        [TestCase("normal string", "normal string", Description = "just a normal string")]
        [TestCase("\u0081","", Description = "unicode escaped char in string should be removed")]
        [TestCase("\u0081ff", "ff", Description = "unicode escaped char in string should be removed")]
        public void ShouldReplaceEscapedUnicodeChars(string inputstring, string outputstring)
        {
            var replacer = new ValueReplacer();
            var replaced = replacer.Replace(inputstring);
            Assert.That(replaced, Is.EqualTo(outputstring));
        }
    }
}
