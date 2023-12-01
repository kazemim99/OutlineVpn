using System;
using System.Globalization;
using System.Reflection;

namespace V2Ray.Api.Extensions
{
    public static class PersianDateExtensionMethods
    {
        private static CultureInfo _Culture;



        public static long GigaByteToBytes(this int gig)
        {
            return Convert.ToInt64(gig * 1024d * 1024d * 1024d);
        }

        public static double ByteToGigaByte(this long bytes)
        { 
        var result = bytes / 1024d / 1024d / 1024d;
            return result;
        }
        public static DateTime TimeStampToDateTime(this long date)
        {

            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(1372061224000 / 1000d)).ToLocalTime();
            return dt;
        }
        public static CultureInfo GetPersianCulture()
        {
            if (_Culture == null)
            {
                _Culture = new CultureInfo("fa-IR");
                DateTimeFormatInfo formatInfo = _Culture.DateTimeFormat;
                formatInfo.AbbreviatedDayNames = new[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
                formatInfo.DayNames = new[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهار شنبه", "پنجشنبه", "جمعه", "شنبه" };
                var monthNames = new[]
                {
                    "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن",
                    "اسفند",
                    ""
                };
                formatInfo.AbbreviatedMonthNames =
                    formatInfo.MonthNames =
                        formatInfo.MonthGenitiveNames = formatInfo.AbbreviatedMonthGenitiveNames = monthNames;
                formatInfo.AMDesignator = "ق.ظ";
                formatInfo.PMDesignator = "ب.ظ";
                formatInfo.ShortDatePattern = "yyyy/MM/dd";
                formatInfo.LongDatePattern = "dddd, dd MMMM,yyyy";
                formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;
                Calendar cal = new PersianCalendar();

                FieldInfo fieldInfo = _Culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                    fieldInfo.SetValue(_Culture, cal);

                FieldInfo info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
                if (info != null)
                    info.SetValue(formatInfo, cal);

                _Culture.NumberFormat.NumberDecimalSeparator = "/";
                _Culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
                _Culture.NumberFormat.NumberNegativePattern = 0;
            }
            return _Culture;
        }

        public static DateTime ToGeo(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(date.Year, date.Month, date.Day,date.Hour,date.Minute,date.Second, pc);
            return dt;
        }
        public static DateTime ToGeo(this string input)
        {
            var date = input.Split("/");
            var tt = DateTime.UtcNow;
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(date[2]),tt.Hour,tt.Minute,tt.Second, pc);
            return dt;
        }
            public static DateTime ToPersianDate(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new(pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date), pc.GetHour(date), pc.GetMinute(date), pc.GetSecond(date),pc);
            return dt;
        }


        public static string ToPeString(this DateTime date, string format = "yyyy/MM/dd")
        {
            return date.ToString(format, GetPersianCulture());
        }
      
        public static long  ToTimeStamp(this DateTime dateTime)
        {
            var epoch = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
            epoch = Convert.ToInt64(epoch+"754");
            return epoch;
        }

        public static DateTime ToDateTime(this long stamp)
        {
            var time = Convert.ToInt64(stamp.ToString().Substring(0 ,stamp.ToString().Length - 3));
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(time).ToLocalTime();
            return dateTime;
        }
    }
}