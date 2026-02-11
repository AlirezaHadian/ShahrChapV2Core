using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShahrChap.Core.Security
{
    public class FileUploadValidatior
    {
        private readonly string[] _allowedExtensions =
{
        ".jpg", ".jpeg", ".png", ".pdf", ".zip", ".rar", ".psd", ".tiff"
    };
        private readonly string[] _allowedMimeTypes =
{
        "image/jpeg",
        "image/png",
        "application/pdf",
        "application/zip",
        "application/x-rar-compressed",
        "image/tiff",
        "image/vnd.adobe.photoshop"
    };

        private const long MaxFileSize = 50 * 1024 * 1024;

        private readonly Dictionary<string, byte[]> _fileSignatures = new()
        {
            [".jpg"] = new byte[] { 0xFF, 0xD8 },
            [".jpeg"] = new byte[] { 0xFF, 0xD8 },
            [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47 },
            [".pdf"] = new byte[] { 0x25, 0x50, 0x44, 0x46 }
        };

        public FileValidationResult Validate(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return FileValidationResult.Fail("فایل نامعتبر است.");

            if (file.Length > MaxFileSize)
                return FileValidationResult.Fail("حجم فایل بیشتر از حد مجاز است.");

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(extension))
                return FileValidationResult.Fail("پسوند فایل مجاز نیست.");

            if (!_allowedMimeTypes.Contains(file.ContentType))
                return FileValidationResult.Fail("نوع فایل (MIME) نامعتبر است.");

            if (_fileSignatures.ContainsKey(extension) && !IsValidSignature(file, extension))
                return FileValidationResult.Fail("ساختار فایل با پسوند آن همخوانی ندارد.");

            return FileValidationResult.Success();
        }


        private bool IsValidSignature(IFormFile file, string extension)
        {
            using var reader = new BinaryReader(file.OpenReadStream());
            var signature = _fileSignatures[extension];
            var headerBytes = reader.ReadBytes(signature.Length);
            return headerBytes.SequenceEqual(signature);
        }

        public string GenerateSafeFileName(string originalFileName)
        {
            var ext = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}{ext}";
        }
        public class FileValidationResult
        {
            public bool IsValid { get; private set; }
            public string ErrorMessage { get; private set; }

            private FileValidationResult(bool isValid, string errorMessage = null)
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }

            public static FileValidationResult Success()
                => new(true);

            public static FileValidationResult Fail(string message)
                => new(false, message);
        }
    }
}
