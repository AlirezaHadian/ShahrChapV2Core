using ShahrChap.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.User;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Security;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.Generators;
using ShahrChap.DataLayer.Entities.Address;
using ShahrChap.DataLayer.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ShahrChap.Core.Services
{
    public class UserService : IUserService
    {
        private ShahrChapContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(ShahrChapContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

        public int GetUserIdWithUserName(string username)
        {
            return _context.Users.Single(u => u.UserName == username).UserId;
        }

        private string AddProfileImage(IFormFile profileImage)
        {
            string avatarName = NameGenerator.GenerateUniqCode() + Path.GetExtension(profileImage.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", avatarName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                profileImage.CopyTo(stream);
            }
            return avatarName;
        }

        private void DeleteProfileImage(string currentAvatarName)
        {
            if (currentAvatarName != "DefaultAvatar.jpg")
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar",
                    currentAvatarName);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
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
                Wallet = BalanceUserWallet(username)
            };
            return informationUser;
        }

        public SideBarUserPanelViewMode GetSideBarUserPanelData(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new SideBarUserPanelViewMode()
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
            var user = GetUserWithUserName(username);

            if (profile.UserAvatar != null)
            {
                DeleteProfileImage(profile.CurrentAvatarName);
                user.UserAvatar = AddProfileImage(profile.UserAvatar);
            }

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

            //user.UserAvatar = profile.CurrentAvatarName;
            UpdateUser(user);
        }

        public bool CompareOldPassword(string oldPassword, string username)
        {
            string hashOldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            return _context.Users.Any(u => u.UserName == username && hashOldPassword == u.Password);
        }

        public void ChangePassword(string username, string newPassword)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            _context.Update(user);
            _context.SaveChanges();
        }

        public int BalanceUserWallet(string username)
        {
            int userId = GetUserIdWithUserName(username);
            var deposit = _context.Wallets.Where(w => w.UserId == userId && w.WalletTypeId == 1 && w.IsPay).Select(w => w.Amount);
            var withdrawal = _context.Wallets.Where(w => w.UserId == userId && w.WalletTypeId == 2 && w.IsPay).Select(w => w.Amount);
            return (deposit.Sum() - withdrawal.Sum());
        }

        public List<WalletViewModel> GetWalletDetailUser(string username)
        {
            int userId = GetUserIdWithUserName(username);
            return _context.Wallets.Where(w => w.UserId == userId && w.IsPay).Select(w => new WalletViewModel()
            {
                Amount = w.Amount,
                DateTime = w.CreateDate,
                Type = w.WalletTypeId,
                Descirption = w.Description
            }).ToList();
        }

        public int ChargeWallet(string username, int amount, string description, bool isPay = false)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                Description = description,
                UserId = GetUserIdWithUserName(username),
                CreateDate = DateTime.Now,
                IsPay = isPay,
                WalletTypeId = 1
            };
            return AddWallet(wallet);
        }

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public Wallet GetWalletWithWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public UserAddress GetUserAddressWithAddressId(int userAddressId)
        {
            return _context.UserAddresses.SingleOrDefault(a => a.UserAddressId == userAddressId);
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

        public User GetUserWithId(int userId)
        {
            return _context.Users.Find(userId);
        }

        public User GetUserWithEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public User GetUserWithPhoneNumber(string phoneNumber)
        {
            return _context.Users.SingleOrDefault(u => u.Phone == phoneNumber);
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

        public List<ShowAddressViewModel> GetUserAdresses(string username)
        {
            int userId = GetUserIdWithUserName(username);
            return _context.UserAddresses.Where(u => u.UserId == userId).Select(u => new ShowAddressViewModel()
            {
                AddressId = u.UserAddressId,
                City = u.City.CityName,
                Province = u.Province.ProvinceName,
                FullAddress = u.FullAddress,
                FullName = u.FullName,
                PostCode = u.PostCode,
                HouseNumber = u.HouseNumber
            }).ToList();
        }

        public List<Province> GetAllProvince()
        {
            return _context.Provinces.ToList();
        }

        public List<City> GetProvincCities(int provinceId)
        {
            return _context.City.Where(c => c.ProvinceId == provinceId).ToList();
        }

        public City GetCityWithCityId(int cityId)
        {
            return _context.City.Find(cityId);
        }

        public Province GetProvinceWithProvinceId(int provinceId)
        {
            return _context.Provinces.Find(provinceId);
        }

        public void AddAddress(UserAddress address)
        {
            _context.UserAddresses.Add(address);
            _context.SaveChanges();
        }

        public bool UpdateAddress(UserAddress address)
        {
            try
            {
                _context.UserAddresses.Update(address);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAddress(UserAddress address)
        {
            try
            {
                address.IsDelete = true;
                UpdateAddress(address);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public UserForAdminViewModel GetUsers(int pageId = 1, string filterUser = "")
        {
            IQueryable<User> result = _context.Users;

            if (!string.IsNullOrEmpty(filterUser))
            {
                result = result.Where(u =>
                    u.Email.Contains(filterUser) || u.Phone.Contains(filterUser) || u.UserName.Contains(filterUser));
            }

            int take = 20;
            int skip = (pageId - 1) * take;

            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();
            return list;
        }

        public int AddUserForAdmin(CreateUserViewModel user)
        {
            User addUser = new User()
            {
                Password = PasswordHelper.EncodePasswordMd5(user.Password),
                Email = user.Email,
                Phone = user.Phone,
                UserName = user.UserName,
                RegisterDate = DateTime.Now,
                ActiveCode = NameGenerator.GenerateUniqCode()
            };
            if (user.Email != null)
                addUser.IsEmailActive = true;
            if (user.Phone != null)
                addUser.IsPhoneActive = true;
            #region Save Avatar
            if (user.UserAvatar != null)
            {
                addUser.UserAvatar = AddProfileImage(user.UserAvatar);
            }
            else
            {
                addUser.UserAvatar = "DefaultAvatar.jpg";
            }
            #endregion
            return AddUser(addUser);
        }

        public EditUserViewModel GetUserForShowInEditMode(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId).Select(u => new EditUserViewModel()
            {
                UserId = u.UserId,
                Email = u.Email,
                Phone = u.Phone,
                UserName = u.UserName,
                CurrentUserAvatar = u.UserAvatar,
                UserRoles = u.UserRoles.Select(r => r.RoleId).ToList()
            }).Single();
        }

        public void EditUserForAdmin(EditUserViewModel editUser)
        {
            User user = GetUserWithId(editUser.UserId);
            if (!string.IsNullOrEmpty(editUser.Email))
            {
                if (user.Email == null)
                {
                    user.IsEmailActive = true;
                }
                user.Email = editUser.Email;
            }
            if (!string.IsNullOrEmpty(editUser.Phone))
            {
                if (user.Phone == null)
                {
                    user.IsPhoneActive = true;
                }
                user.Phone = editUser.Phone;
            }
            

            if (!string.IsNullOrEmpty(editUser.Password))
            {
                user.Password = PasswordHelper.EncodePasswordMd5(editUser.Password);
            }

            if (editUser.UserAvatar != null)
            {
                DeleteProfileImage(editUser.CurrentUserAvatar);

                user.UserAvatar = AddProfileImage(editUser.UserAvatar);
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public UserForAdminViewModel GetDeleteUsers(int pageId = 1, string filterUser = "")
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u=> u.IsDelete);

            if (!string.IsNullOrEmpty(filterUser))
            {
                result = result.Where(u =>
                    u.Email.Contains(filterUser) || u.Phone.Contains(filterUser) || u.UserName.Contains(filterUser));
            }

            int take = 20;
            int skip = (pageId - 1) * take;

            UserForAdminViewModel list = new UserForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();
            return list;
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserWithId(userId);
            user.IsDelete = true;
            UpdateUser(user);
        }

        public InformationUserViewModel GetUserInformation(int userId)
        {
            var user = GetUserWithId(userId);
            InformationUserViewModel informationUser = new InformationUserViewModel()
            {
                Email = user.Email,
                Phone = user.Phone,
                UserName = user.UserName,
                RegisterDate = user.RegisterDate,
                Wallet = BalanceUserWallet(user.UserName)
            };
            return informationUser;
        }

        public string GetCurrentUserRole()
        {
            int userId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            if (userId == null || userId == 0)
            {
                return null; 
            }

            string userRoleTitle = _context.UserRoles.Where(u=> u.UserId == userId).Select(u=> u.Role.RoleTitle).FirstOrDefault();
            return userRoleTitle;
        }
    }
}
