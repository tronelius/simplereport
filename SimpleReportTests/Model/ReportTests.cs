using NUnit.Framework;
using SimpleReport.Model;

namespace SimpleReportTests.Model
{
    [TestFixture]
    public class ReportTests
    {

        [Test]
        public void ShouldBeTreatedAsMasterDetailReportSql()
        {
            Report rpt = new Report() { Sql = "select '1' as merge_id, 'header' as header from table1; select '2' as merge_id, 'detail' as detail from table2" };
            Assert.That(rpt.IsMasterDetailReport(), Is.True);
        }

        [Test]
        public void ShouldNotBeTreatedAsMasterDetailReportSql()
        {
            Report rpt = new Report() { Sql = "select '1' as one, 'header' as header from table1; select '2' as two, 'detail' as detail from table2" };
            Assert.That(rpt.IsMasterDetailReport(), Is.False);

        }
    }
}
