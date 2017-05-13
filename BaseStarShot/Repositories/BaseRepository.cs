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
    public abstract class BaseRepository
    {
        public static ISQLitePlatform SqlitePlatform;

        protected SQLiteAsyncConnection dbConn;

		protected BaseRepository(string dbPath, bool storeDateTimeAsTicks = false, IBlobSerializer serializer = null)
        {
            //initialize a new SQLiteConnection 
            if (dbConn == null)
            {
                var connectionFunc = new Func<SQLiteConnectionWithLock>(() =>
                    new SQLiteConnectionWithLock
                    (
                        SqlitePlatform,
                        new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: storeDateTimeAsTicks)
                    ));

                dbConn = new SQLiteAsyncConnection(connectionFunc);
            }
        }

        protected async Task CreateTableAsync<T>()
            where T : class
        {
            bool createSuccess = false;
            try
            {
                await dbConn.CreateTableAsync<T>();
                createSuccess = true;
            }
            catch (SQLite.Net.SQLiteException ex)
            {
                BaseStarShot.Logger.WriteError("BaseRepository", ex);
            }

            if (!createSuccess)
            {
                await dbConn.DropTableAsync<T>();
                await dbConn.CreateTableAsync<T>();
            }
        }
    }
}
