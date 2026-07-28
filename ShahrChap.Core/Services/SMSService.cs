using Microsoft.Extensions.Options;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Options;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ShahrChap.Core.Services
{
    public class SMSService:ISMSService
    {
        private readonly HttpClient _httpClient;
        private readonly string _username;
        private readonly string _password;
        private readonly string _token;
        private readonly int _otpPatternCodeId;
        private readonly ISessionManager _sessionManager;
        private readonly IUserService _userService;

        public SMSService(IOptions<MessageSenderOptions> options, ISessionManager sessionManager, IUserService userService)
        {
            _username = options.Value.UserName;
            _password = options.Value.Password;
            _token = options.Value.Token;
            _otpPatternCodeId = options.Value.OtpPatternCodeId;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://portal.amootsms.com/rest")
            };

            _sessionManager = sessionManager;
            _userService = userService;
        }

        public async Task<string> SendQuickOtpAsync(string mobile, short codeLength = 4, string optionalCode = "")
        {
            var form = new Dictionary<string, string>
        {
            { "Token", _token },
            { "Mobile", mobile },
            { "CodeLength", codeLength.ToString() },
            { "OptionalCode", optionalCode }
        };

            var content = new FormUrlEncodedContent(form);
            var resp = await _httpClient.PostAsync("SendQuickOTP", content);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync();
        }

        public async Task<string> SendWithPatternAsync(string mobile, int patternCodeId, string[] patternValues)
        {
            var form = new Dictionary<string, string>
        {
            { "Token", _token },
            { "Mobile", mobile },
            { "PatternCodeID", patternCodeId.ToString() },
            { "PatternValues", string.Join(",", patternValues) }
        };

            var content = new FormUrlEncodedContent(form);
            var resp = await _httpClient.PostAsync("SendWithPattern", content);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync();
        }


        //        public static void SendWithPattern(string phone, string code)
        //        {
        //            var client = new AmootSMS.WebService2SoapClient(AmootSMS.WebService2SoapClient.EndpointConfiguration.WebService2Soap12,
        //"https://portal.amootsms.com/webservice2.asmx");
        //            string UserName = _username;
        //            string Password = _password;
        //            string Mobile = phone;
        //            int PatternCodeID = _otpPatternCodeId;
        //            string[] PatternValues = new string[] { code };

        //            AmootSMS.WebService2SoapClient webService = client;

        //            Task<AmootSMS.SendResult> result = client.SendWithPatternAsync(UserName, Password, Mobile, PatternCodeID, PatternValues);
        //        }

        //        public void SendOtpCode(string? phonenumber)
        //        {
        //            //Clearing the oldest otp's 
        //            _sessionManager.Remove("OtpCode");
        //            _sessionManager.Remove("OtpExpireTime");

        //            string userPhone;
        //            if (phonenumber != null)
        //                userPhone = phonenumber;
        //            else
        //                userPhone = _sessionManager.Get("UserPhone");

        //            User user = _userService.GetUserWithPhoneNumber(userPhone);
        //            //If send otp was from posting register, the userphone will get from session
        //            //If send otp was from resend code button in verify phone, 

        //            string OtpCode = NameGenerator.GenerateOTP();
        //            MessageSender.SendWithPattern(userPhone, OtpCode);

        //            _sessionManager.Set("OtpCode", OtpCode);
        //            _sessionManager.Set("OtpExpireTime", DateTime.Now.AddSeconds(120).ToString());
        //            _sessionManager.Set("UserPhone", user.Phone);
        //        }
    }
}