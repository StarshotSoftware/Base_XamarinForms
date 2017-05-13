using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class Product
    {
		public double Price { get; set; }
        public string FormattedPrice { get; set;}
        public string ProductName { get; set;}
        public string ProductId { get; set;}
        public ProductType Type { get; set;}
    }
}
