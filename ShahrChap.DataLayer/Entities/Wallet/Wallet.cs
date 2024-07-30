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
    [Required(ErrorMessage = "{0} اجباری می باشد")]
    public int WalletTypeId { get; set; }
    [Display(Name = "کاربر")]
    [Required(ErrorMessage = "{0} اجباری می باشد")]
    public int UserId { get; set; }
    [Display(Name = "مبلغ")]
    [Required(ErrorMessage = "{0} اجباری می باشد")]
    public int Amount { get; set; }
    [Display(Name = "شرح")]
    [MaxLength(500,ErrorMessage = "")]
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