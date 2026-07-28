using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Options
{
    public class MessageSenderOptions
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public int OtpPatternCodeId { get; set; }
    }
}
