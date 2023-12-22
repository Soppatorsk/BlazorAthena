using System.ComponentModel.DataAnnotations;

namespace BlazorAthenaFrontend.Data
{
    public class SubCategory
    {
        [Key]
        public int ID { get; set; } 
        [Required]
        public string Name { get; set; }

    }
}
