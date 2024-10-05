using ShahrChap.DataLayer.Entities.Permissions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.User
{
    public class Role
    {
        public Role()
        {
            
        }

        [Key]
        public int RoleId { get; set; }

        [Display(Name = "عنوان دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string RoleTitle { get; set; }
        public bool IsDelete { get; set; }

        #region Relation
        public virtual List<UserRole>? UserRoles { get; set; }
        public virtual List<RolePermission>? RolePermissions {  get; set; }
        #endregion
    }
}
