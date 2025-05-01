using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class CartDisplayItem
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public byte[] ImageData { get; set; }
    }
}
