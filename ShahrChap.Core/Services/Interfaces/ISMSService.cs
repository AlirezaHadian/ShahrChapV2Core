using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface ISMSService
    {
        Task<string> SendQuickOtpAsync(string mobile, short codeLength = 4, string optionalCode = "");
        Task<string> SendWithPatternAsync(string mobile, int patternCodeId, string[] patternValues);
    }
}
