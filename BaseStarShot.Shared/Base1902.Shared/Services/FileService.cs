using Base1902.IO;
using BaseStarShot.IO;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class FileService : IFileService
    {
        const int ClearImageExpirationInSeconds = 10;
        public readonly string separtor = ForPlatform.Get("/", "/", "\\", "\\", "\\");
        static AutoResetEvent clearImageCacheEvent = new AutoResetEvent(true);
        static DateTime lastClearDate;

        public virtual async Task SaveAsync(File file)
        {
#if !WINDOWS_PHONE_APP && !WINDOWS_APP
            if (!System.IO.Directory.Exists(file.Folder))
                System.IO.Directory.CreateDirectory(file.Folder);
#endif
            var folderPath = file.Folder;
            IFolder folder = await FileSystem.Current.GetFolderFromPathAsync(folderPath);
#if WINDOWS_PHONE_APP || WINDOWS_APP
            Stack<string> folders = new Stack<string>();

            while (folder == null && folderPath.Contains(separtor))
            {
                var folderPathArray = folderPath.Split(new String[] { separtor }, StringSplitOptions.RemoveEmptyEntries);
              folderPath = string.Join(separtor, folderPathArray.Take(folderPathArray.Length - 1));
              folders.Push(folderPathArray.Last());
              folder = await FileSystem.Current.GetFolderFromPathAsync(folderPath);
            }

            while (folders.Count >0)
            {
              folder =  await folder.CreateFolderAsync(folders.Pop(), CreationCollisionOption.OpenIfExists);
            }
#endif

            IFile f = await folder.CreateFileAsync(file.Name, CreationCollisionOption.OpenIfExists);

            byte[] buffer = await file.GetDataAsync();
            using (System.IO.Stream stream = await f.OpenAsync(FileAccess.ReadAndWrite))
            {
				await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        public virtual async Task<File> LoadAsync(string filepath)
        {
            IFile file = await FileSystem.Current.GetFileFromPathAsync(filepath);

            return file.ToFile();
        }
        public virtual async Task<bool> Exists(string filePath)
        {
            return await FileSystem.Current.GetFileFromPathAsync(filePath) != null;
        }

        public virtual async Task ClearImageCache()
        {
            await Task.Run(async () =>
            {
                if (!clearImageCacheEvent.WaitOne(5000))
                    return;
                try
                {
                    if (lastClearDate.AddSeconds(ClearImageExpirationInSeconds) < DateTime.Now)
                    {
                        IFolder folder = await FileSystem.Current.LocalStorage.GetFolderAsync("ImageLoaderCache");
                        if (folder != null)
                        {
                            await folder.DeleteAsync();
                            lastClearDate = DateTime.Now;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    clearImageCacheEvent.Set();
                }
            });
        }
    }
}
