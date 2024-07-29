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

        public InformationUserViewModel GetUserInformation(string username)
        {
            var user = GetUserWithUserName(username);
            InformationUserViewModel informationUser = new InformationUserViewModel()
            {
                Email = user.Email,
                Phone = user.Phone,
                UserName = user.UserName,
                RegisterDate = user.RegisterDate,
                Wallet = 0
            };
            return informationUser;
        }

        public SideBarUserPanelViewMode GetSideBarUserPanelData(string username)
        {
            return _context.Users.Where(u=> u.UserName==username).Select(u=> new SideBarUserPanelViewMode()
            {
                UserName = u.UserName,
                ImageName = u.UserAvatar,
                RegisterDate = u.RegisterDate
            }).Single();
        }

        public EditProfileViewModel GetDataForEditProfileUser(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new EditProfileViewModel()
            {
                UserName = u.UserName,
                Email = u.Email,
                Phone = u.Phone,
                CurrentAvatarName = u.UserAvatar
            }).Single();
        }

        public void EditProfile(string username, EditProfileViewModel profile)
        {
            if (profile.UserAvatar != null)
            {
                string imagePath;
                if (profile.CurrentAvatarName != "DefaultAvatar.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar",
                        profile.CurrentAvatarName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
                profile.CurrentAvatarName = NameGenerator.GenerateUniqCode() + Path.GetExtension(profile.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar",
                    profile.CurrentAvatarName);

                using var stream = new FileStream(imagePath, FileMode.Create);
                profile.UserAvatar.CopyTo(stream);
            }

            var user = GetUserWithUserName(username);
            user.UserName = profile.UserName;
            if (user.Email != null && user.Email != profile.Email)
            {
                user.Email = profile.Email;
                user.IsEmailActive = false;
            }
            if (user.Phone != null && user.Phone != profile.Phone)
            {
                user.Phone = profile.Phone;
                user.IsPhoneActive = false;
            }
            
            user.UserAvatar = profile.CurrentAvatarName;
            UpdateUser(user);
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

        public User GetUserWithUserName(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
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
