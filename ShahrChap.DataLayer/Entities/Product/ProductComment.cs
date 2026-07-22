using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Product
{
    public class ProductComment
    {
        [Key]
        public int CommentID { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public int? ParentID { get; set; }
        [DisplayName("متن نظر")]
        [Required(ErrorMessage = "لطفا (0) را وارد کنید.")]
        [MaxLength(700,ErrorMessage = "تعداد کاراکتر های مجاز حداکثر (1) می‌باشد.")]
        public string Text { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? EditDate { get; set; }
        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        #region Relations
        public virtual Product Product { get; set; }
        public virtual ShahrChap.DataLayer.Entities.User.User User { get; set; }
        public virtual ICollection<ProductComment> Replies { get; set; }
        #endregion
    }
}
