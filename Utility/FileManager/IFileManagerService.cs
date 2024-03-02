using Microsoft.AspNetCore.Http;

namespace Utility.FileManager
{
    public interface IFileManagerService
    {
        void DeleteFile(string filePath);
        Task<string> UploadFile(IFormFile file, string folderName);
    }
}
