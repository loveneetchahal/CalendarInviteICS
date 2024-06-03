using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Data.SqlTypes;
using System.Security.Claims;
using System.Text;
using System;
using System.Text.RegularExpressions;
using TimeZoneConverter;
using Ical.Net.Serialization;
using Ical.Net.CalendarComponents;
using Ical.Net;
using Ical.Net.DataTypes;

namespace CalendarInviteICS.Helpers
{
    public static class CommonFunctions
    {
        public static readonly char stringSplitterPipe = '|';
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        public static List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day)) // Map each day to a date
                             .ToList(); // Load dates into a list
        }

        public static string GetMonthYearDayFromDate(DateTime date)
        {
            DateTime datevalue = date;
            string dy = datevalue.Day.ToString();
            string mn = datevalue.Month.ToString();
            string yy = datevalue.Year.ToString();
            string data = dy + "," + mn + "," + yy;
            return data;
        }
        public static DateTime GetDateTime(string strDate, string strTime)
        {
            DateTime dt;
            int dd = Convert.ToInt32(strDate.Substring(0, 2));
            int mm = Convert.ToInt32(strDate.Substring(2, 2));
            int yy = Convert.ToInt32("20" + strDate.Substring(4, 2));
            TimeSpan ts = GetTimeFromString(strTime);
            dt = new DateTime(yy, mm, dd, ts.Hours, ts.Minutes, 0);
            return dt;
        }

        public static TimeSpan GetTimeFromString(string strTime)
        {
            int hh = Convert.ToInt32(strTime.Substring(0, 2));
            int mins = Convert.ToInt32(strTime.Substring(3, 2));
            int ss = Convert.ToInt32(strTime.Substring(6, 2));
            return new TimeSpan(hh, mins, ss);
        }
        public static int GetTimeDifferenceInMinutes(DateTime fromTime, DateTime toTime)
        {
            TimeSpan ts = fromTime - toTime;
            return Convert.ToInt32(ts.TotalMinutes);
        }
        public static Ical.Net.Calendar CreateICSCalendar(DateTime? start, DateTime? end, string title, string description, string location, string organizername, string organizermail)
        {
            var calendar = new Ical.Net.Calendar();
            calendar.Method = "PUBLISH";
            CalendarEvent evt = calendar.Create<CalendarEvent>();
            evt.Class = "PUBLIC";
            evt.Summary = title;
            evt.Created = new CalDateTime(DateTime.Now);
            evt.Description = description;
            //evt.Start = new CalDateTime(DateTime.ParseExact(start.Value.ToString(format: "MM/dd/yyyy HH:mm"), "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)); 
            //evt.End = new CalDateTime(DateTime.ParseExact(end.Value.ToString(format: "MM/dd/yyyy HH:mm"), "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)); 
            evt.Start = new CalDateTime(ConvertToVCalendarDateString(start.Value));
            evt.End = new CalDateTime(ConvertToVCalendarDateString(end.Value));
            evt.Sequence = 0;
            evt.Uid = Guid.NewGuid().ToString();
            evt.Location = location;
            evt.Transparency = TransparencyType.Transparent;
            evt.Organizer = new Organizer()
            {
                CommonName = organizername,
                Value = new Uri($"mailto:{organizermail}")
            };
            Alarm reminder = new Alarm();
            reminder.Action = AlarmAction.Display;
            reminder.Trigger = new Trigger(new TimeSpan(-2, 0, 0));
            evt.Alarms.Add(reminder);
            return calendar;
        }

        public static System.Net.Mail.Attachment GetICSAttachment(Ical.Net.Calendar calendarContent, string fileName)
        {
            var serializer = new CalendarSerializer(new SerializationContext());
            var serializedCalendar = serializer.SerializeToString(calendarContent);
            var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);
            MemoryStream ms = new MemoryStream(bytesCalendar);
            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "Event_" + fileName + ".ics", "text/calendar");
            return attachment;
        }

        public static string Generate13UniqueDigits()
        {
            return DateTime.Now.ToString("yyMMddHHmmssf");
        }

        public static string Generate15UniqueDigits()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssf");
        }

        //16 digit timestamp
        public static string GetShortTimestamp()
        {
            return DateTime.Now.ToString("yyMMddHHmmssffff");
        }

        //18 digit timestamp
        public static string GetLongTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        public static string ConvertToVCalendarDateString(DateTime d)
        {
            d = d.ToUniversalTime();
            string yy = d.Year.ToString();
            string mm = d.Month.ToString("D2");
            string dd = d.Day.ToString("D2");
            string hh = d.Hour.ToString("D2");
            string mm2 = d.Minute.ToString("D2");
            string ss = d.Second.ToString("D2");
            string s = yy + mm + dd + "T" + hh + mm2 + ss + "Z"; // Pass date as vCalendar format YYYYMMDDTHHMMSSZ (includes middle T and final Z) '
            return s;
        }

        public static DateTime ConvertUtctoUserTimeZoneById(string timezone, DateTime datetime)
        {
            timezone = TZConvert.IanaToWindows(timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(datetime, TimeZoneInfo.FindSystemTimeZoneById(timezone));
        }
    }
}
