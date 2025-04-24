using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesOrderApp.Models
{
    public class SO_ORDER
    {
        public long SO_ORDER_ID { get; set; }
        public string ORDER_NO { get; set; } = string.Empty;
        public DateTime ORDER_DATE { get; set; }
        public int COM_CUSTOMER_ID { get; set; }
        public string ADDRESS { get; set; } = string.Empty;

        public COM_CUSTOMER? Customer { get; set; }
        public List<SO_ITEM> Items { get; set; } = new List<SO_ITEM>();
    }
}