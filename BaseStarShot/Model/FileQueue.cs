using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace BaseStarShot.Model
{
    public class FileQueue
    {
        [PrimaryKey]
        public Guid UploadOperationUID { get; set; }
        public Guid UID { get; set; }
        public bool IsComplete { get; set; }
        public bool IsCancelled { get; set; }
    }
}
