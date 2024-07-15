using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ShahrChap.Core.Senders
{
    public class MessageSender
    {
        public static void SendWithPattern(string phone, string username, string code)
        {
            var client = new AmootSMS.WebService2SoapClient(AmootSMS.WebService2SoapClient.EndpointConfiguration.WebService2Soap12,
"https://portal.amootsms.com/webservice2.asmx");
            string UserName = "09397673794";
            string Password = "1274255325";
            string Mobile = phone;
            int PatternCodeID = 2078;
            string[] PatternValues = new string[] { username, code };

            AmootSMS.WebService2SoapClient webService = client;

            Task<AmootSMS.SendResult> result = client.SendWithPatternAsync(UserName, Password, Mobile, PatternCodeID, PatternValues);
        }
    }
}
