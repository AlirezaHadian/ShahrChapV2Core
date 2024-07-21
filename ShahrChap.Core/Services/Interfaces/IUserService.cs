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
        User GetUserWithEmail(string email);
        User GetUserWithPhoneNumber(string phoneNumber);
        User GetUserWithActiveCode(string activeCode);
        void UpdateUser(User user);
        bool ActiveEmail(string activeCode);
        bool ActivePhone(string phoneNumber);
    }
}
