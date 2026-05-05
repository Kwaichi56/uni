using System;
using System.Collections.Generic;
using System.Text;

namespace WPFShop.Models
{
    public class Product
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public string Category { get; set; } // Протеин, Креатин, Витамины
        public double Rating { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Дополнительные параметры
        public string Flavor { get; set; } // Вкус
        public double Weight { get; set; } // Вес (вместо размера)
        public string CountryOrigin { get; set; } // Страна доставки/производства
        public decimal Discount { get; set; }
        public bool IsOutOfStock => Quantity == 0;
        public int SoldCount { get; set; }
        public string Manufacturer { get; set; }
    }
}
