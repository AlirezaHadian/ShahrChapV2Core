using Microsoft.AspNetCore.Http;
using ShahrChap.DataLayer.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShahrChap.Core.DTOs;

public class UserForAdminViewModel
{
    public List<User> Users { get; set; }
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
}

public class CreateUserViewModel
{
    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(250, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    public string UserName { get; set; }

    [Display(Name = "شماره موبایل")]
    [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [RegularExpression(@"(00989|\+989|989|09)(\d{9})", ErrorMessage = "{0} معتبر نمی باشد")]
    public string? Phone { get; set; }
    [Display(Name = "ایمیل")]
    [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [RegularExpression(@"([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})", ErrorMessage = "{0} معتبر نمی باشد")]
    public string? Email { get; set; }
    [Display(Name = "کلمه عبور")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public IFormFile? UserAvatar { get; set; }
}

public class EditUserViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; }

    [Display(Name = "شماره موبایل")]
    [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [RegularExpression(@"(00989|\+989|989|09)(\d{9})", ErrorMessage = "{0} معتبر نمی باشد")]
    public string? Phone { get; set; }
    [Display(Name = "ایمیل")]
    [MaxLength(450, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [RegularExpression(@"([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})", ErrorMessage = "{0} معتبر نمی باشد")]
    public string? Email { get; set; }
    [Display(Name = "کلمه عبور")]
    [MaxLength(50, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    public IFormFile? UserAvatar { get; set; }
    public List<int>? UserRoles { get; set; }
    public string CurrentUserAvatar { get; set; }
}