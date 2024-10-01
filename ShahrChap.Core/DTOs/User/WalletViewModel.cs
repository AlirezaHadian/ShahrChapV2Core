using System.ComponentModel.DataAnnotations;

namespace ShahrChap.Core.DTOs;

public class ChargeWalletViewModel
{
    [Display(Name = "مبلغ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int Amount { get; set; }
}

public class WalletViewModel
{
    public int Amount { get; set; }
    public int Type { get; set; }
    public string Descirption { get; set; }
    public DateTime DateTime { get; set; }
}