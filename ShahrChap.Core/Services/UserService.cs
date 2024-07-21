using ShahrChap.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.User;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Security;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.Generators;

namespace ShahrChap.Core.Services
{
    public class UserService : IUserService
    {
        ShahrChapContext _context;
        public UserService(ShahrChapContext context)
        {
            _context = context;
        }
        public bool ActiveEmail(string activeCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
            if (user == null || user.IsEmailActive)
                return false;

            user.IsEmailActive = true;
            user.ActiveCode = NameGenerator.GenerateUniqCode();
            _context.SaveChanges();
            return true;
        }

        public bool ActivePhone(string phoneNumber)
        {
            var user = _context.Users.SingleOrDefault(u => u.Phone == phoneNumber);
            if (user == null || user.IsPhoneActive) return false;

            user.IsPhoneActive = true;
            _context.SaveChanges();
            return true;
        }

        public int AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public User GetUserWithActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public User GetUserWithEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public User GetUserWithPhoneNumber(string phoneNumber)
        {
            return _context.Users.SingleOrDefault(u=> u.Phone == phoneNumber);
        }

        public bool IsEmailOrPhoneExist(string emailOrPhone)
        {
            return _context.Users.Any(u => u.Email == emailOrPhone || u.Phone == emailOrPhone);
        }

        public bool IsUserNameExist(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        public User LoginUser(LoginViewModel login)
        {
            string hashPassword = PasswordHelper.EncodePasswordMd5(login.Password);
            string emailOrPhone = FixText.FixEmail(login.EmailOrPhone);
            if (login.EmailOrPhone.Contains("@"))
            {
                return _context.Users.SingleOrDefault(u => u.Email == emailOrPhone && u.Password == hashPassword);
            }
            else
            {
                return _context.Users.SingleOrDefault(u => u.Phone == emailOrPhone && u.Password == hashPassword);
            }
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
