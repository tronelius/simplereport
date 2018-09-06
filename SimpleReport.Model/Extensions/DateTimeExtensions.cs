﻿using System;
using System.Globalization;

namespace SimpleReport.Model.Extensions 
{
  
    public static class DateTimeExtensions
    {
        const int JAN = 1;
        const int DEC = 12;
        const int LASTDAYOFDEC = 31;
        const int FIRSTDAYOFJAN = 1;
        const int THURSDAY = 4;


        /// <summary>
        /// Get the first date of the month for a certain date.
        /// </summary>
        public static DateTime GetFirstDateOfMonth(this DateTime date)
        {
            return date == DateTime.MinValue ? date : new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Get the first date of the week for a certain date.
        /// </summary>
        /// <param name="date">DateTime instance.</param>
        /// <param name="iso8601">Whether or not the date is an ISO 8601 date; by default false.</param>
        /// <param name="weekRule">The week rule to use; by default CalendarWeekRule.FirstFourDayWeek.</param>
        /// <param name="firstDayOfWeek">Which day that is the first day of the week; by defaul DayOfWeek.Monday.</param>
        /// <returns>The first date of the week for the date.</returns>
        public static DateTime GetFirstDateOfWeek(this DateTime date, bool iso8601 = false, CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
        {
            if (date == DateTime.MinValue)
                return date;

            var week = date.GetWeekNumber(iso8601, weekRule, firstDayOfWeek);
            while (week == date.GetWeekNumber(iso8601, weekRule, firstDayOfWeek))
                date = date.AddDays(-1);
            return date.AddDays(1);
        }

        /// <summary>
        /// Get the last date of the month for a certain date.
        /// </summary>
        public static DateTime GetLastDateOfMonth(this DateTime date)
        {
            return date == DateTime.MaxValue ? date : new DateTime(date.Year, date.Month, 1).AddMonths(1);
        }

        /// <summary>
        /// Get the last date of the week for a certain date.
        /// 
        /// Note that for ISO 8601 dates, iso8601 must be set to true.
        /// </summary>
        public static DateTime GetLastDateOfWeek(this DateTime date, bool iso8601 = false, CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
        {
            if (date == DateTime.MaxValue)
                return date;

            var week = date.GetWeekNumber(iso8601, weekRule, firstDayOfWeek);
            while (week == date.GetWeekNumber(iso8601, weekRule, firstDayOfWeek))
                date = date.AddDays(1);
            return date;
        }

        /// <summary>
        /// Get the week number of a certain date.
        /// 
        /// Note that for ISO 8601 dates, iso8601 must be set to true.
        /// </summary>
        public static int GetWeekNumber(this DateTime date, bool iso8601 = false, CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
        {
            return iso8601 ? GetWeekNumber_Iso8601(date) : CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
        }

        /// <summary>
        /// Get the week number of a certain ISO 8601 date.
        /// </summary>
        private static int GetWeekNumber_Iso8601(this DateTime date)
        {
            //Get the day number since the beginning of the year
            var dayOfYear = date.DayOfYear;

            //Get the first and last weekday of the year
            var startWeekDayOfYear = (int)(new DateTime(date.Year, JAN, FIRSTDAYOFJAN)).DayOfWeek;
            var endWeekDayOfYear = (int)(new DateTime(date.Year, DEC, LASTDAYOFDEC)).DayOfWeek;

            //Compensate for using monday as the first day of the week
            if (startWeekDayOfYear == 0)
                startWeekDayOfYear = 7;
            if (endWeekDayOfYear == 0)
                endWeekDayOfYear = 7;

            //Calculate the number of days in the first week
            var daysInFirstWeek = 8 - (startWeekDayOfYear);

            //Year starting and ending on a thursday will have 53 weeks
            var thursdayFlag = (startWeekDayOfYear == THURSDAY || endWeekDayOfYear == THURSDAY);

            //We begin by calculating the rounded up number of FULL weeks between the year start and our date.
            var resultWeekNumber = (int)Math.Ceiling((dayOfYear - (daysInFirstWeek)) / 7.0);

            //If the first week of the year has at least four days, the week number can be incremented by one.
            if (daysInFirstWeek >= THURSDAY)
                resultWeekNumber = resultWeekNumber + 1;

            //If the week number is larger than 52 (and the year doesn't start/end on a thursday), the week number is 1.
            if (resultWeekNumber > 52 && !thursdayFlag)
                resultWeekNumber = 1;

            //If the week number is still 0, it we are trying to evaluate the week number for a week that belongs to the
            //previous year (since it has 3 days or less in this year). We therefore execute this function recursively.
            if (resultWeekNumber == 0)
                resultWeekNumber = GetWeekNumber(new DateTime(date.Year - 1, DEC, LASTDAYOFDEC));
            return resultWeekNumber;
        }

        /// <summary>
        /// Check if two DateTime instances represent the same date, regardless of the time.
        /// </summary>
        public static bool IsSameDate(this DateTime date, DateTime compareDate)
        {
            return date.Date == compareDate.Date;
        }


        //Adds to Saidis extensions.
        public static int Quarter(this DateTime date)
        {
            return (date.Month + 2) / 3;
        }

        public static DateTime GetFirstDayOfQuarter(this DateTime date)
        {
            int quarter = date.Quarter();
            return new DateTime(date.Year, (quarter-1)*3+1, 1);
        }

        public static DateTime GetLastDayOfQuarter(this DateTime date)
        {
            DateTime quarterstart = date.GetFirstDayOfQuarter();
            return quarterstart.AddMonths(3);
        }

        public static DateTime GetFirstDayOfLastQuarter(this DateTime date)
        {
            return date.GetFirstDayOfQuarter().AddMonths(-3);
        }

        public static DateTime GetLastDayOfLastQuarter(this DateTime date)
        {
            return date.GetLastDayOfQuarter().AddMonths(-3);
        }

        public static DateTime GetFirstDayOfYear(this DateTime date)
        {
            return new DateTime(date.Year,1,1);
        }

        public static DateTime GetLastDayOfYear(this DateTime date)
        {
            return new DateTime(date.Year+1, 1, 1);
        }

        public static DateTime GetFirstDayOfLastYear(this DateTime date)
        {
            return date.GetFirstDayOfYear().AddYears(-1);
        }

        public static DateTime GetLastDayOfLastYear(this DateTime date)
        {
            return date.GetLastDayOfYear().AddYears(-1);
        }
        
    }
}