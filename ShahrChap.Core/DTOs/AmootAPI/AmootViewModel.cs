using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.DTOs.AmootAPI
{
    public class SendSmsRequest
    {
        public string Token { get; set; }
        public string LineNumber { get; set; }
        public string SMSMessageText { get; set; }
        public string[] Mobiles { get; set; }
        // اگر خواستی زمان ارسال هم بدهی:
        public DateTime SendDateTime { get; set; }
    }
}
