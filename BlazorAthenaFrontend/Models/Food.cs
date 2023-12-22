using BlazorAthena.Models;
using System.ComponentModel.DataAnnotations;

namespace BlazorAthenaFrontend.Models
{
    public class Food
    {
        [Key]
        public int ID { get; set; }
        public bool Lactose { get; set; } = false;
        public bool Nuts { get; set; } = false;
    }
}
