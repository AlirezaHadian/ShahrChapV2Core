using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IFileStorageService
    {
        string SaveTempFile(int cartItemId, IFormFile file);
        void MoveToPermanent(int cartItemId, string fileName, int orderFileId);
        void DeleteTempFile(int cartItemId, string fileName);
        void DeletePermanentFile(int orderFileId, string fileName);
        void CleanupEmptyTempFolder(int cartItemId);
        string GetTempFilePath(int cartItemId, string fileName);
        string GetPermanentFilePath(int orderFileId, string fileName);
    }
}
