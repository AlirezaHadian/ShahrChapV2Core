using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface ISessionManager
    {
        void Set(string key, string value);
        string Get(string key);
        void Remove(string key);
    }
}
