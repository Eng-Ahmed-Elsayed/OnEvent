using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Utility.FileManager
{
    public class FileManagerService : IFileManagerService
    {

        /// <summary>
        /// Upload new file to the db and retrun the db path for the file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<string> UploadFile(IFormFile file, string folderName)
        {
            folderName = Path.Combine("wwwroot", "UploadedFiles", folderName);
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            return await SaveFile(file, pathToSave, folderName);
        }

        /// <summary>
        /// Save the file to the server and return the dbPath to save it in the database.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="pathToSave"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private async Task<string> SaveFile(IFormFile file, string pathToSave, string folderName)
        {
            var fileName = Guid.NewGuid().ToString() + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return await Task.FromResult(dbPath.Split("wwwroot\\")[1]);
        }
        /// <summary>
        /// Delete file with full path.
        /// </summary>
        /// <param name="imgPath"></param>
        public void DeleteFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                // Delete with full path
                //File.Delete(Path.Combine(Directory.GetCurrentDirectory(), filePath));
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath));
            }
        }
    }
}
