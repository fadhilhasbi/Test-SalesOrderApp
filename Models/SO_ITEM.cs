using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesOrderApp.Models
{
    public class SO_ITEM
    {
        public long SO_ITEM_ID { get; set; }
        public long SO_ORDER_ID { get; set; }
        public string ITEM_NAME { get; set; } = string.Empty;
        public int QUANTITY { get; set; }
        public double PRICE { get; set; }
    }
}