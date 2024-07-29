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