using S7Test.Core.Model.Enum;
using System.ComponentModel.DataAnnotations;

namespace S7Test.Web.Models
{
    public class PlayerViewModel
    {       
        public int Id { get; set; }
        [Required]
        public bool IsActive { get; set; }

        [StringLength(2048, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Picture { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public int YellowCards { get; set; }
        [Required]
        public int RedCards { get; set; }
        [Required]
        public int Goals { get; set; }
        [Required]
        public int Appearances { get; set; }
        [Required]
        [StringLength(1024, ErrorMessage = "The {0} must be at most {1} characters long.")]
        public string Name { get; set; }
        public Gender? Gender { get; set; }

        //Navigation        
        public TeamViewModel Team { get; set; }
        
    }
}
