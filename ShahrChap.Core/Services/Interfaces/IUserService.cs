using ShahrChap.Core.DTOs;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IUserService
    {
        bool IsUserNameExist(string userName);
        bool IsEmailOrPhoneExist(string emailOrPhone);
        int AddUser(User user);
        User LoginUser(LoginViewModel login);
        bool ActiveEmail(string activeCode);
        bool ActivePhone(string PhoneNumber);
    }
}
