using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAthena.Models
{
    public class Order
    {
        [Key]
        public int ID { get; set; }

        // Remove the OrderLineID property from here

        public string? Comment { get; set; }
        public bool Accepted { get; set; } = false;
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public string? KitchenComment { get; set; }
        public bool Delivered { get; set; } = false;
        public decimal? SaleAmount { get; set; } = decimal.Zero;

        // Add a collection of OrderLines
        public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
        public string? UserID { get; set; } = "0";
    }
}
