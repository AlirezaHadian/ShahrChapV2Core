using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IEmailService
    {
        void Send(string to, string subject, string body);
    }
}
