using Microsoft.AspNetCore.Http;
using ShahrChap.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Services
{
    public class SessionManager:ISessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Set(string key, string value)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(key, value);
        }

        public string Get(string key)
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(key);
        }

        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext?.Session.Remove(key);
        }
    }
}
