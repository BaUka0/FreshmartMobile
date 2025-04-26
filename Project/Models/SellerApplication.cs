using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class SellerApplication
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        [Ignore]
        public User User { get; set; }
        [Ignore]
        public bool IsPending => Status == "Pending";
    }
}