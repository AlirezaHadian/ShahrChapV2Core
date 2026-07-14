using Microsoft.Identity.Client;
using ShahrChap.DataLayer.Entities.Address;
using ShahrChap.DataLayer.Entities.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.DTOs.Cart
{
    public class CartForPopoverViewModel
    {
        public bool HasItems => CartItems.Any();

        public List<CartItemForPopoverViewModel> CartItems { get; set; } = [];

        public decimal TotalPrice { get; set; }
    }

    public class CartItemForPopoverViewModel
    {
        public int CartItemID { get; set; }
        public int ProductId { get; set; }

        public string ProductTitle { get; set; }

        public string ImageName { get; set; }

        public long Price { get; set; }
    }

    public class CartDetailsViewModel
    {
        public ShahrChap.DataLayer.Entities.Cart.Cart Cart { get; set; }
        public List<CartItemViewModel> Items { get; set; } = [];

        public List<AddressForCartViewModel> UserAddresses { get; set; } = [];

        public decimal TotalPrice { get; set; }
    }

    public class CartItemViewModel
    {
        public int CartItemID { get; set; }
        public int OrderDetailID { get; set; }
        public string ProductTitle { get; set; }
        public string OrderTitle { get; set; }
        public List<string> ProductFeatures { get; set; }
        public List<string> SelectedFeaturesValue { get; set; }
        public List<string>? Services { get; set; } = new();
        public List<FileViewModel> Files { get; set; }
        public decimal FinalPrice { get; set; }
    }

    public class FileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
    }
}
