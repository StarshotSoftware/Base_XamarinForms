using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseStarShot.Model;

namespace BaseStarShot.Repositories
{
    public interface IFileQueueRepository
    {
        Task<List<FileQueue>> GetFileQueueListAsync();
        Task<FileQueue> GetFileQueueAsync(Guid UploadOperationUID);
        Task<bool> SaveFileQueueAsync(FileQueue file);
    }
}
