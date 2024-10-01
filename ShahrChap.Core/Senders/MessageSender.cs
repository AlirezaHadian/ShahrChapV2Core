using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;

namespace ShahrChap.Core.Senders
{
    public class MessageSender
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IUserService _userService;

        public MessageSender(IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _contextAccessor = contextAccessor;
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
        
        public static void SendOtpCode(string? phonenumber, IUserService userService, IHttpContextAccessor contextAccessor)
        {
            //Clearing the oldest otp's sessions
            contextAccessor.HttpContext.Session.Remove("OtpCode");
            contextAccessor.HttpContext.Session.Remove("OtpExpireTime");

            string userPhone;
            if (phonenumber != null)
                userPhone = phonenumber;
            else
                userPhone = contextAccessor.HttpContext.Session.GetString("UserPhone");

            User user = userService.GetUserWithPhoneNumber(userPhone);
            //If send otp was from posting register, the userphone will get from session
            //If send otp was from resend code button in verify phone, 

            string OtpCode = NameGenerator.GenerateOTP();
            MessageSender.SendWithPattern(userPhone, OtpCode);
            contextAccessor.HttpContext.Session.SetString("OtpCode", OtpCode);
            contextAccessor.HttpContext.Session.SetString("OtpExpireTime", DateTime.Now.AddMinutes(2).ToString());
            contextAccessor.HttpContext.Session.SetString("UserPhone", user.Phone);
        }
    }
}
