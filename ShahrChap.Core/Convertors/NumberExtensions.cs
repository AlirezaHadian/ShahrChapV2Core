using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ShahrChap.Core.Convertors
{
    public static class NumberExtensions
    {
        private static readonly char[] PersianDigits =
        { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };

        public static string ToPersianPrice(this decimal value)
        {
            var number = value.ToString("N0");
            return string.Concat(number.Select(c =>
                char.IsDigit(c) ? PersianDigits[c - '0'] : c));
        }
    }
}
