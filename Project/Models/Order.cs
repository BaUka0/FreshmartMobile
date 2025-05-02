using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public string Address { get; set; }
        public string TotalPrice { get; set; }
        public string OrderStatus { get; set; } // "Обработка", "Доставляется", "Доставлено"

        [Ignore]
        public List<OrderItem> Items { get; set; }
    }
}
