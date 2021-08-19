using System;

namespace PersonalAPI.Extensions
{
    public static class DateTimeExtensions
    {
        public static readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static readonly string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Gets the 12:00:00 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteStart(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteEnd(this DateTime dateTime)
        {
            return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
        }
    }
}
