using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.DTOs.Products
{
    public class MainMenuViewModel
    {
        public int GroupId { get; set; }

        public string GroupTitle { get; set; } = string.Empty;

        public string? IconClass { get; set; }

        public List<MainMenuSubGroupViewModel> SubGroups { get; set; } = new();
    }

    public class MainMenuSubGroupViewModel
    {
        public int GroupId { get; set; }

        public string GroupTitle { get; set; } = string.Empty;

        public List<MainMenuProductViewModel> Products { get; set; } = new();
    }

    public class MainMenuProductViewModel
    {
        public int ProductId { get; set; }

        public string ProductTitle { get; set; } = string.Empty;
    }
}
