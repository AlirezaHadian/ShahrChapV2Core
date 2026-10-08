using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IOtpService
    {
        string GeneratePhoneOtp(string mobile, string actionType);
        bool VerifyPhoneOtp(string mobile, string otp);
        string? GetPhone();
        string? GetActionType();
        void RemoveOtp();
    }
}
