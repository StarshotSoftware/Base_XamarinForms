using BaseStarShot.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStarShot.Repositories
{
    public class ErrorRepository<T> : BaseNonAsyncRepository, BaseStarShot.Repositories.IErrorRepository
        where T : ErrorData, new()
    {
        public ErrorRepository() : base(System.IO.Path.Combine(BaseStarShot.Globals.DatabasesFolder, "error.db3"))
        {
            CreateTable<T>();
        }

        public List<ErrorData> GetAll()
        {
            return dbConnNonAsync.Table<T>().Cast<ErrorData>().ToList();
        }

        public List<ErrorData> GetAllPending()
        {
            return dbConnNonAsync.Table<T>().Where(e => e.IsSent == false).Cast<ErrorData>().ToList();
        }

        public bool Save(ErrorData error)
        {
            if (error.Id == 0 || dbConnNonAsync.Update(error) == 0)
            {
                return dbConnNonAsync.Insert(error) > 0;
            }
            return true;
        }
    }
}
