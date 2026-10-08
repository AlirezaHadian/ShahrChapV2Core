using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IOtpStorage
    {
        void Save(string phone, string otp, DateTime expireTime, string actionType);
        string? GetOtp(string phone);
        DateTime? GetExpireTime(string phone);
        string? GetPhone();
        string? GetActionType();
        void Remove();
    }
}
