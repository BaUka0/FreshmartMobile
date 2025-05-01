using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public byte[] ImageData { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; } = 1;


        [Ignore]
        public bool IsFavoriteButtonVisible { get; set; } = true;
        [Ignore]
        public bool IsCartButtonVisible { get; set; } = true;
        [Ignore]
        public string FavoriteIcon { get; set; } = "favourite_grey.png";


    }

}