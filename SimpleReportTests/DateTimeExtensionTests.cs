using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleReport.Model.Extensions;

namespace SimpleReportTests
{
    [TestFixture]
    public class DateTimeExtensionTests
    {

        [Test]
        public void ShouldCalculateQuarters()
        {
            DateTime time = new DateTime(2016,2,2);
            Assert.That(time.GetFirstDayOfQuarter(), Is.EqualTo(new DateTime(2016,1,1)));
            Assert.That(time.GetLastDayOfQuarter(), Is.EqualTo(new DateTime(2016, 4, 1)));
            Assert.That(time.GetFirstDayOfLastQuarter(), Is.EqualTo(new DateTime(2015, 10, 1)));
            Assert.That(time.GetLastDayOfLastQuarter(), Is.EqualTo(new DateTime(2016, 1, 1)));   
        }

        [Test]
        public void ShouldCalculateYears()
        {
            DateTime time = new DateTime(2016, 2, 2);
            Assert.That(time.GetFirstDayOfYear(), Is.EqualTo(new DateTime(2016, 1, 1)));
            Assert.That(time.GetLastDayOfYear(), Is.EqualTo(new DateTime(2017, 1, 1)));
            Assert.That(time.GetFirstDayOfLastYear(), Is.EqualTo(new DateTime(2015, 1, 1)));
            Assert.That(time.GetLastDayOfLastYear(), Is.EqualTo(new DateTime(2016, 1, 1)));
        }

        [Test]
        public void ShouldCalculateMonth()
        {
            DateTime time = new DateTime(2016, 2, 2);
            Assert.That(time.GetFirstDateOfMonth(), Is.EqualTo(new DateTime(2016, 2, 1)));
            Assert.That(time.GetLastDateOfMonth(), Is.EqualTo(new DateTime(2016, 3, 1)));
        }

        [Test]
        public void ShouldCalculateWeek()
        {
            DateTime time = new DateTime(2016, 2, 2); //week 5
            Assert.That(time.GetFirstDateOfWeek(), Is.EqualTo(new DateTime(2016, 2, 1)));
            Assert.That(time.GetLastDateOfWeek(), Is.EqualTo(new DateTime(2016, 2, 8)));

        }
    }
}
