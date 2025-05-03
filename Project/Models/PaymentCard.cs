using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class PaymentCard
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryDate { get; set; }
        public bool IsDefault { get; set; }

        // Маскированный номер карты для отображения
        [Ignore]
        public string MaskedCardNumber => CardNumber.Length > 4
            ? $"**** **** **** {CardNumber.Substring(CardNumber.Length - 4)}"
            : CardNumber;
    }
}
