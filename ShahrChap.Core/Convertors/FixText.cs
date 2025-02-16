using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShahrChap.Core.Convertors
{
    public class FixText
    {
        public static string FixEmail(string email)
        {
            return email.Trim().ToLower();
        }
        public static string GetPlainText(string html)
        {
            return Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}
