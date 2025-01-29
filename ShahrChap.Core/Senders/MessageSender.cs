using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;
using static System.Net.WebRequestMethods;

namespace ShahrChap.Core.Senders
{
    public class MessageSender
    {
        private readonly ISessionManager _sessionManager;
        private readonly IUserService _userService;

        public MessageSender(ISessionManager sessionManager, IUserService userService)
        {
            _sessionManager = sessionManager;
            _userService = userService;
        }
        public static void SendWithPattern(string phone, string code)
        {
            var client = new AmootSMS.WebService2SoapClient(AmootSMS.WebService2SoapClient.EndpointConfiguration.WebService2Soap12,
"https://portal.amootsms.com/webservice2.asmx");
            string UserName = "09397673794";
            string Password = "1274255325";
            string Mobile = phone;
            int PatternCodeID = 2638;
            string[] PatternValues = new string[] { code };

            AmootSMS.WebService2SoapClient webService = client;

            Task<AmootSMS.SendResult> result = client.SendWithPatternAsync(UserName, Password, Mobile, PatternCodeID, PatternValues);
        }
        
        public void SendOtpCode(string? phonenumber)
        {
            //Clearing the oldest otp's 
            _sessionManager.Remove("OtpCode");
            _sessionManager.Remove("OtpExpireTime");

            string userPhone;
            if (phonenumber != null)
                userPhone = phonenumber;
            else
                userPhone = _sessionManager.Get("UserPhone");

            User user = _userService.GetUserWithPhoneNumber(userPhone);
            //If send otp was from posting register, the userphone will get from session
            //If send otp was from resend code button in verify phone, 

            string OtpCode = NameGenerator.GenerateOTP();
            MessageSender.SendWithPattern(userPhone, OtpCode);

            _sessionManager.Set("OtpCode", OtpCode);
            _sessionManager.Set("OtpExpireTime", DateTime.Now.AddSeconds(120).ToString());
            _sessionManager.Set("UserPhone", user.Phone);
        }
    }
}
