using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public byte[] ProfileImage { get; set; }
        public string role { get; set; } = "client"; // "admin", "sales" or "client"
        public bool CanDelete => !string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase);
    }
}
