using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.DataLayer.Entities.Permissions
{
    public class Permission
    {
        public Permission()
        {

        }
        [Key]
        public int PermissionId { get; set; }
        [Display(Name = "عنوان دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(250, ErrorMessage = "{0}  نمی تواند بیش از {1} کاراکتر باشد")]
        public string PermissionTitle { get; set; }
        public int? ParentId { get; set; }
        
        #region Relations
        [ForeignKey("ParentId")]
        public virtual List<Permission> Permissions { get; set; }
        public virtual List<RolePermission> RolePermissions { get; set; }
        #endregion
    }
}
