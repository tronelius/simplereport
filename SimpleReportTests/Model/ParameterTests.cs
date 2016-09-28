using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SimpleReport.Model;
using SimpleReport.Model.DbExecutor;
using SimpleReport.Model.Replacers;

namespace SimpleReportTests
{
    [TestFixture]
    public class ParameterTests
    {
        public string SQL { get; set; }

        [SetUp]
        public void Setup()
        {
            SQL = String.Empty;
        }

        [TestCase("Select * from t where (@inclause is null or t.val in (@inclause))","Select * from t where (@inclause is null or t.val in (@inclause,@inclause1,@inclause2))")]
        [TestCase("Select * from t where t.val in (@inclause)", "Select * from t where t.val in (@inclause,@inclause1,@inclause2)")]
        [TestCase("Select * from t where t.val in(@inclause)", "Select * from t where t.val in(@inclause,@inclause1,@inclause2)")]
        [TestCase("Select * from t where t.val = @incluse or t.val in (@inclause)", "Select * from t where t.val = @incluse or t.val in (@inclause,@inclause1,@inclause2)")]
        public void ShouldChange_In_ParameterValues(string inputSQL, string expectedOutputSQL)
        {
            var par = GetParameterWithInClauseForTest();
            DbSqlServerExecutor exec = new DbSqlServerExecutor(new NoReplacer());
            var parameterList = par.GetDbParameterForLookup(inputSQL, update, exec);
            Assert.That(parameterList.Count, Is.EqualTo(3));
            Assert.That(SQL, Is.EqualTo(expectedOutputSQL));
        }

        private void update(string obj)
        {
            SQL = obj;
        }

        private static Parameter GetParameterWithInClauseForTest()
        {
            Parameter par = new Parameter();
            par.SqlKey = "@inclause";
            par.InputType = ParameterInputType.LookupMultipleChoice;
            par.Value = "111,112,113";
            return par;
        }
    }
}
