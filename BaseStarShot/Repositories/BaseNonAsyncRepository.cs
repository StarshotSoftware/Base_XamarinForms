using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Repositories
{
    public abstract class BaseNonAsyncRepository : BaseRepository
    {
        protected SQLiteConnection dbConnNonAsync;

        public BaseNonAsyncRepository(string dbPath, bool storeDateTimeAsTicks = false, IBlobSerializer serializer = null)
            : base(dbPath, storeDateTimeAsTicks: storeDateTimeAsTicks, serializer: serializer)
        {
            //initialize a new SQLiteConnection 
            if (dbConnNonAsync == null)
            {
                dbConnNonAsync = new SQLiteConnection(BaseRepository.SqlitePlatform, dbPath, storeDateTimeAsTicks: storeDateTimeAsTicks);
            }
        }

        protected void CreateTable<T>()
            where T : new()
        {
            bool createSuccess = false;
            try
            {
                dbConnNonAsync.CreateTable<T>();
                createSuccess = true;
            }
            catch (SQLite.Net.SQLiteException ex)
            {
                BaseStarShot.Logger.WriteError("BaseNonAsyncRepository", ex);
            }

            if (!createSuccess)
            {
                dbConnNonAsync.DropTable<T>();
                dbConnNonAsync.CreateTable<T>();
            }
        }
    }
}
