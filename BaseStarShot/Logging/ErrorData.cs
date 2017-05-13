using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Logging
{
    public class ErrorData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Guid ErrorId { get; set; }

        public string UserAgent { get; set; }

        public string Exception { get; set; }

        public DateTime DateTime { get; set; }

        public double DateTimeOffset { get; set; }

        public bool IsSent { get; set; }

        public DateTime? DateTimeSent { get; set; }

        public double? DateTimeSentOffset { get; set; }

        public virtual void OnOnAdditionalData(StringBuilder errorString)
        {

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var dt = new DateTimeOffset(DateTime.AddHours(DateTimeOffset), TimeSpan.FromHours(DateTimeOffset));
            sb.AppendLine(dt.ToString("yyyy-MM-dd HH:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture));
            OnOnAdditionalData(sb);
            sb.AppendLine("User agent: " + UserAgent);
            sb.AppendLine();
            sb.AppendLine(Exception);

            return sb.ToString();
        }
    }
}
