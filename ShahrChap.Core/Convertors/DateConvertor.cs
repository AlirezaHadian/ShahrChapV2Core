using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Convertors
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime time)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(time) + "/" + pc.GetMonth(time).ToString("00")+ "/" + pc.GetDayOfMonth(time);
        }

        public static string ToRelativePersianTime(this DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;

            if (span.TotalSeconds < 60)
                return "چند لحظه پیش";

            if (span.TotalMinutes < 60)
                return $"{ToPersianDigits((int)span.TotalMinutes)} دقیقه پیش";

            if (span.TotalHours < 24)
                return $"{ToPersianDigits((int)span.TotalHours)} ساعت پیش";

            if (span.TotalDays < 7)
                return $"{ToPersianDigits((int)span.TotalDays)} روز پیش";

            if (span.TotalDays < 30)
                return $"{ToPersianDigits((int)(span.TotalDays / 7))} هفته پیش";

            if (span.TotalDays < 365)
                return $"{ToPersianDigits((int)(span.TotalDays / 30))} ماه پیش";

            // خیلی قدیمی -> تاریخ کامل شمسی
            return dateTime.ToPersianDateString();
        }
        public static string ToPersianDateString(this DateTime dateTime)
        {
            var pc = new PersianCalendar();
            string result = $"{pc.GetYear(dateTime)}/{pc.GetMonth(dateTime):00}/{pc.GetDayOfMonth(dateTime):00}";
            return ToPersianDigits(result);
        }
        private static string ToPersianDigits(object value)
        {
            string input = value.ToString();
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            var result = new System.Text.StringBuilder();
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                    result.Append(persianDigits[c - '0']);
                else
                    result.Append(c);
            }
            return result.ToString();
        }
    }
}
