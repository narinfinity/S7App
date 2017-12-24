using System.ComponentModel.DataAnnotations;

namespace S7Test.Web.Models
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(128, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Name { get; set; }
    }
}
