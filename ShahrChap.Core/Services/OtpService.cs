using ShahrChap.Core.Generators;
using ShahrChap.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpStorage _otpStorage;
        public OtpService(IOtpStorage otpStorage)
        {
            _otpStorage = otpStorage;
        }
        public string GeneratePhoneOtp(string mobile, string actionType)
        {
            string otp = NameGenerator.GenerateOTP();

            _otpStorage.Save(
                mobile,
                otp,
                DateTime.Now.AddMinutes(2),
                actionType);

            return otp;
        }

        public bool VerifyPhoneOtp(string mobile, string otp)
        {
            string? savedOtp = _otpStorage.GetOtp(mobile);
            DateTime? expireTime = _otpStorage.GetExpireTime(mobile);

            if(savedOtp == null || expireTime == null) return false;

            if (savedOtp != otp) return false;

            if(expireTime < DateTime.Now)
            {
                _otpStorage.Remove();
                return false;
            }

            _otpStorage.Remove();
            return true;
        }
        public string? GetPhone()
        {
            return _otpStorage.GetPhone();
        }
        public string? GetActionType()
        {
            return _otpStorage.GetActionType();
        }
        public void RemoveOtp()
        {
            _otpStorage.Remove();
        }
        public DateTime? GetExpireTime()
        {
            string? phone = _otpStorage.GetPhone();

            if (string.IsNullOrWhiteSpace(phone))
                return null;

            return _otpStorage.GetExpireTime(phone);
        }
    }
}
