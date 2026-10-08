using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Infrastructure
{
    public class SessionOtpStorage : IOtpStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionOtpStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private ISession Session =>
            _httpContextAccessor.HttpContext!.Session;
        public void Save(string phone, string otp, DateTime expireTime, string actionType)
        {
            Session.SetString("OtpCode", otp);
            Session.SetString("OtpPhone", phone);
            Session.SetString("OtpExpireTime", expireTime.ToString("O"));
            Session.SetString("OtpActionType", actionType);
        }

        public string? GetOtp(string phone)
        {
            var savedPhone = Session.GetString("OtpPhone");

            if (savedPhone != phone)
                return null;

            return Session.GetString("OtpCode");
        }

        public DateTime? GetExpireTime(string phone)
        {
            var savedPhone = Session.GetString("OtpPhone");
            if(savedPhone != phone)
                return null;

            var value = Session.GetString("OtpExpireTime");

            if (string.IsNullOrEmpty(value))
                return null;

            if (DateTime.TryParse(value, out DateTime expireTime))
                return expireTime;

            return null;
        }
        public string? GetPhone()
        {
            return Session.GetString("OtpPhone");
        }
        public string? GetActionType()
        {
            return Session.GetString("OtpActionType");
        }
        public void Remove()
        {
            Session.Remove("OtpCode");
            Session.Remove("OtpPhone");
            Session.Remove("OtpExpireTime");
            Session.Remove("OtpActionType");
        }
    }
}
