using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Enums
{
    public enum FormCreationState
    {
        [Display(Name = "بدون فرم")]
        NoFormAllowed,
        [Display(Name ="آپلود فایل")]
        FileUploadOnly,
        [Display(Name = "طراحی اختصاصی")]
        CustomDesignOnly,
        [Display(Name = "آپلود فایل + طراحی اختصاصی")]
        BothFormsAllowed,
    }
}
