using System.ComponentModel.DataAnnotations;

namespace BlazorAthenaFrontend.Models
{
    public class SubCategory
 
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
