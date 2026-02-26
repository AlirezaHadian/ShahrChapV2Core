using System.ComponentModel.DataAnnotations;

namespace ShahrChap.DataLayer.Enums
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
