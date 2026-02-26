using System.ComponentModel.DataAnnotations;

namespace ShahrChap.DataLayer.Enums
{
    public enum OrderPaymentStatus
    {
        [Display(Name ="سبد خرید")]
        Cart = 0,

        [Display(Name = "در انتظار پرداخت")]
        PendingPayment = 1,

        [Display(Name = "پرداخت شده")]
        Paid = 2,

        [Display(Name = "پرداخت ناموفق")]
        PaymentFailed = 3,

        [Display(Name = "تکلمیل شده")]
        Completed = 4
    }
}
