using BlazorAthena.Models;
using System.ComponentModel.DataAnnotations;

namespace AthenaResturantWebAPI.Models
{
    public class Merch
    {
        [Key]
        public int ID { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }
}
