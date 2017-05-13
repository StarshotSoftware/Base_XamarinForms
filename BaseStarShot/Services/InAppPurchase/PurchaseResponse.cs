using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class PurchaseResponse
    {
        public string TransactionId { get; set; }
        
        public string ProductId { get; set; }
        
        public string Receipt { get; set; }

        public PurchaseStatus Status { get; set; }

    }
}
