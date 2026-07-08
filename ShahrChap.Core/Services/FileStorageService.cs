using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Core.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;
        private const string TempRoot = "Uploads/Temp";
        private const string PermanentRoot = "Uploads/Files";
        public FileStorageService(IWebHostEnvironment env)
        {
            _webRootPath = env.WebRootPath;
        }
        public string SaveTempFile(int cartItemId, IFormFile file)
        {
            string folder = GetTempFolder(cartItemId);
            Directory.CreateDirectory(folder);

            string uniqueName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(folder, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                file.CopyTo(stream);

            return uniqueName;
        }

        public void MoveToPermanent(int cartItemId, string fileName, int orderFileId)
        {
            string sourcePath = Path.Combine(GetTempFolder(cartItemId), fileName);
            string destFolder = GetPermanentFolder(orderFileId);
            Directory.CreateDirectory(destFolder);
            string destPath = Path.Combine(destFolder, fileName);

            if (File.Exists(sourcePath))
                File.Move(sourcePath, destPath, overwrite: true);
        }

        public void DeleteTempFile(int cartItemId, string fileName)
        {
            string path = Path.Combine(GetTempFolder(cartItemId), fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        public void DeletePermanentFile(int orderFileId, string fileName)
        {
            string path = Path.Combine(GetPermanentFolder(orderFileId), fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        public void CleanupEmptyTempFolder(int cartItemId)
        {
            string folder = GetTempFolder(cartItemId);
            if (Directory.Exists(folder) && !Directory.EnumerateFileSystemEntries(folder).Any())
                Directory.Delete(folder);
        }

        private string GetTempFolder(int cartItemId) =>
            Path.Combine(_webRootPath, TempRoot, $"Cart_{cartItemId}");

        private string GetPermanentFolder(int orderFileId) =>
            Path.Combine(_webRootPath, PermanentRoot, $"File_{orderFileId}");

        public string GetTempFilePath(int cartItemId, string fileName) =>
    Path.Combine(GetTempFolder(cartItemId), fileName);

        public string GetPermanentFilePath(int orderFileId, string fileName) =>
            Path.Combine(GetPermanentFolder(orderFileId), fileName);
    }
}
