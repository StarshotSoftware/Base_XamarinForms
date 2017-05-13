using Base1902.IO;
using BaseStarShot.IO;
using System;
using System.Threading.Tasks;
namespace BaseStarShot.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Saves file.
        /// </summary>
        /// <param name="file"></param>
        Task SaveAsync(File file);

        /// <summary>
        /// Loads file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        Task<File> LoadAsync(string filepath);

        /// <summary>
        /// Clears all image cache.
        /// </summary>
        /// <returns></returns>
        Task ClearImageCache();

        
        /// <summary>
        /// Check if file exist
        /// </summary>
        /// <returns></returns>
        Task<bool> Exists(string filePath);
    }
}
