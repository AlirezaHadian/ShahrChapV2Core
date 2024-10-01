using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ShahrChap.Core.DTOs;

public class InformationUserViewModel
{
    public string UserName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime RegisterDate { get; set; }
    public int Wallet { get; set; }
}

public class SideBarUserPanelViewMode
{
    public string UserName { get; set; }
    public DateTime RegisterDate { get; set; }
    public string ImageName { get; set; }
}

public class EditProfileViewModel
{
    public string UserName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public IFormFile? UserAvatar { get; set; }
    public string CurrentAvatarName { get; set; }
}

public class ChangePasswordViewModel
{
    public string oldPassword { get; set; }
    [Display(Name = "کلمه عبور")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [DataType(DataType.Password)]
    public string newPassword { get; set; }
    [Display(Name = "تکرار کلمه عبور")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [Compare("newPassword", ErrorMessage = "کلمه های عبور مطابقت ندارند")]
    [DataType(DataType.Password)]
    public string confirmNewPassword { get; set; }
}