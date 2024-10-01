using System.ComponentModel.DataAnnotations;

namespace ShahrChap.DataLayer.Entities.Wallet;

public class Wallet
{
    public Wallet()
    {
        
    }
    [Key]
    public int WalletId { get; set; }
    [Display(Name = "نوع تراکنش")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int WalletTypeId { get; set; }
    [Display(Name = "کاربر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int UserId { get; set; }
    [Display(Name = "مبلغ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int Amount { get; set; }
    [Display(Name = "شرح")]
    [MaxLength(500, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
    public string Description { get; set; }
    [Display(Name = "وضعیت پرداخت")]
    public bool IsPay { get; set; }
    [Display(Name = "تاریخ ثبت")]
    public DateTime CreateDate { get; set; }

    #region Relations
    public virtual User.User User { get; set; }
    public virtual WalletType WalletType { get; set; }
    #endregion
}