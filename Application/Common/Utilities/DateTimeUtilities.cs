using System.Globalization;

namespace Application.Common.Utilities
{
    public static class DateTimeUtilities
    {
        private static string vnTimeZoneString = "SE Asia Standard Time";

        public static TimeZoneInfo GetVnTimeZoneInfo
            => TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneString);

        public static DateTime GetDateTimeVnNow()
        {
            return TimeZoneInfo.ConvertTime(
                DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneString));
        }

        public static DateTime GetTodayDateTimeVn()
        {
            return TimeZoneInfo.ConvertTime(
                DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),
                TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneString));
        }

        public static DateTime ToDateTime(DateOnly date, TimeOnly time)
        {
            return date.ToDateTime(time);
        }

        public static TimeSpan CalculateTripEndTime(TimeSpan beginTime, double duration)
        {
            return beginTime.Add(new TimeSpan(0, (int)Math.Ceiling(duration), 0));
        }

        public static long GetTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        public static long GetTimeStamp()
        {
            return GetTimeStamp(GetDateTimeVnNow());
        }

        public static bool IsInCurrentWeek(this DateTime dateTimeToCheck)
        {
            Calendar calendar = DateTimeFormatInfo
                .CurrentInfo.Calendar;
            DateTime now = GetDateTimeVnNow();
            var currentDate = now.Date.AddDays(-1 * (int)calendar.GetDayOfWeek(now));
            var dateToCheck = dateTimeToCheck.Date.AddDays(-1 * (int)calendar.GetDayOfWeek(dateTimeToCheck));

            return currentDate == dateToCheck;
        }

        public static bool IsInCurrentMonth(this DateTime dateTime)
        {
            DateTime vnNow = GetDateTimeVnNow();
            return vnNow.Month == dateTime.Month &&
                vnNow.Year == dateTime.Year;
        }

        public static DateTimeOffset ToVnDateTimeOffset(this DateTime dateTime)
        {
            DateTimeOffset vnTimeOffset = new DateTimeOffset(dateTime,
                GetVnTimeZoneInfo.GetUtcOffset(dateTime));

            return vnTimeOffset;
        }

        public static IEnumerable<DateOnly> GetCurrentWeekDates()
        {
            DateTime vnNow = GetDateTimeVnNow();
            int currentDayOfWeek = (int)vnNow.DayOfWeek;
            DateTime sunday = vnNow.AddDays(-currentDayOfWeek);
            DateTime monday = sunday.AddDays(1);
            if (currentDayOfWeek == 0)
            {
                monday = monday.AddDays(-7);
            }
            IEnumerable<DateOnly> weekDates = Enumerable.Range(0, 7).Select(days =>
                DateOnly.FromDateTime(monday.AddDays(days))).OrderBy(d => d.Day);
            return weekDates;

        }

        public static IEnumerable<DateOnly> GetCurrentMonthDates()
        {
            DateTime vnNow = GetDateTimeVnNow();
            DateTime startDate = new DateTime(vnNow.Year, vnNow.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            IEnumerable<DateOnly> monthDates = Enumerable.Range(
                0, endDate.Day).Select(days =>
                DateOnly.FromDateTime(startDate.AddDays(days))).OrderBy(d => d.Day);
            return monthDates;
        }
    }
}
