using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Review
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        [Ignore]
        public string UserName { get; set; }
    }
}
