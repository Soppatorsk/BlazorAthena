﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAthena.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Image { get; set; }

        [ForeignKey("Drink")]
        public int? DrinkID { get; set; }

        public Drink? Drink { get; set; }

        [ForeignKey("Food")]
        public int? FoodID { get; set; }

        public Food? Food { get; set; }

        public bool Available { get; set; } = true;

        [ForeignKey("SubCategory")]
        public int SubCategoryId { get; set; }

        public SubCategory SubCategory { get; set; }
    }
}
