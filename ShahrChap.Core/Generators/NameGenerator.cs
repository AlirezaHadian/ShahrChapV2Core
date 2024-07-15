using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Generators
{
    public class NameGenerator
    {
        public static string GenerateUniqCode() => Guid.NewGuid().ToString().Replace("-","");
        public static string GenerateOTP()
        {
            var randomNumber = new Random();
            string otp = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                otp += randomNumber.Next(0, 10).ToString();
            }
            return otp;
        }
    }
}
