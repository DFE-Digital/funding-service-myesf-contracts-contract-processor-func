using System;

namespace Pds.Contracts.ContractEventProcessor.Services.Extensions
{
    /// <summary>
    /// Date Time Extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Extension method to convert a date time into a formatted date string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The formatted date.</returns>
        public static string ToDateDisplay(this DateTime dateTime)
        {
            return dateTime.ToString("dd MMMM yyyy a\\t hh:mmtt");
        }

        /// <summary>
        /// Extension method to convert a GMT datetime to a UTC datetime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>Datetime.</returns>
        public static DateTime ToUtcTime(this DateTime dateTime)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        /// <summary>
        /// Extension method to convert a date time into a formatted date string.
        /// Format will be MMMM yyyy e.g. March 2016.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The formatted date.</returns>
        public static string ToFullMonthAndFullYearDisplay(this DateTime dateTime)
        {
            return dateTime.ToString("MMMM yyyy");
        }
    }
}