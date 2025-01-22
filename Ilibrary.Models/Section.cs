using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ilibrary.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Section Name")]
        public string Name { get; set; } 

    }
}
