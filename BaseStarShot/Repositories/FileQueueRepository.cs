using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BaseStarShot.Model;
using BaseStarShot.Repositories;

namespace BaseStarShot.Repositories
{
    public class FileQueueRepository : BaseRepository, IFileQueueRepository
    {
        static AutoResetEvent getFileQueueEvent = new AutoResetEvent(true);
        static AutoResetEvent saveFileQueueEvent = new AutoResetEvent(true);
        Task createTableTask;

        public FileQueueRepository()
            : base("file_queue")
        {
            createTableTask = Task.Run(async () =>
            {
                await CreateTableAsync<FileQueue>().ConfigureAwait(false);
            });
        }

        public async Task<List<FileQueue>> GetFileQueueListAsync() 
        {
            await createTableTask;
            return await dbConn.Table<FileQueue>().ToListAsync();
        }

        public async Task<FileQueue> GetFileQueueAsync(Guid UploadOperationUID) {
            await createTableTask;
            return await dbConn.Table<FileQueue>().Where(e => e.UploadOperationUID == UploadOperationUID).FirstOrDefaultAsync();
        }

        public async Task<bool> SaveFileQueueAsync(FileQueue entity)
        {
            int result = 0;
            if (saveFileQueueEvent.WaitOne()) {
                try
                {
                    result = await dbConn.InsertOrReplaceAsync(entity);
                }
                finally {
                    saveFileQueueEvent.Set();
                }
            }
            return result > 0;
        }
    }
}
