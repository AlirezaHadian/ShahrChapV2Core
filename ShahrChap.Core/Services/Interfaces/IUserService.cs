using ShahrChap.Core.DTOs;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShahrChap.DataLayer.Entities.Address;
using ShahrChap.DataLayer.Entities.Wallet;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IUserService
    {
        bool IsUserNameExist(string userName);
        bool IsEmailOrPhoneExist(string emailOrPhone);
        int AddUser(User user);
        User LoginUser(LoginViewModel login);
        User GetUserWithId(int userId);
        User GetUserWithEmail(string email);
        User GetUserWithPhoneNumber(string phoneNumber);
        User GetUserWithActiveCode(string activeCode);
        User GetUserWithUserName(string username);
        void UpdateUser(User user);
        bool ActiveEmail(string activeCode);
        bool ActivePhone(string phoneNumber);
        int GetUserIdWithUserName(string username);
        //string AddProfileImage(IFormFile profileImage);
        //void DeleteProfileImage(string currentAvatarName);
        #region  User Panel
        InformationUserViewModel GetUserInformation(string username);
        SideBarUserPanelViewMode GetSideBarUserPanelData(string username);
        EditProfileViewModel GetDataForEditProfileUser(string username);
        void EditProfile(string username, EditProfileViewModel profile);
        bool CompareOldPassword(string oldPassword, string username);
        void ChangePassword(string username, string newPassword);

        #endregion

        #region Wallet
        int BalanceUserWallet(string username);
        List<WalletViewModel> GetWalletDetailUser(string username);
        int ChargeWallet(string username, int amount, string description, bool isPay = false);
        int AddWallet(Wallet wallet);
        Wallet GetWalletWithWalletId(int walletId);
        void UpdateWallet(Wallet wallet);

        #endregion
        #region Address
        UserAddress GetUserAddressWithAddressId(int userAddressId);
        List<ShowAddressViewModel> GetUserAdresses(string username);
        List<Province> GetAllProvince();
        List<City> GetProvincCities(int provinceId);
        City GetCityWithCityId(int cityId);
        Province GetProvinceWithProvinceId(int provinceId);
        void AddAddress(UserAddress address);
        bool UpdateAddress(UserAddress address);
        bool DeleteAddress(UserAddress address);

        #endregion

        #region Admin Panel
        InformationUserViewModel GetUserInformation(int userId);
        UserForAdminViewModel GetUsers(int pageId = 1, string filterUser = "");
        UserForAdminViewModel GetDeleteUsers(int pageId = 1, string filterUser = "");
        int AddUserForAdmin(CreateUserViewModel user);
        EditUserViewModel GetUserForShowInEditMode(int userId);
        void EditUserForAdmin(EditUserViewModel editUser);
        void DeleteUser(int userId);
        string GetCurrentUserRole();
        #endregion
    }
}
