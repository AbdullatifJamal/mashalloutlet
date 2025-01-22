using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilibrary.Models
{
    public class Type
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Type Name")]
        public string ?Name { get; set; }
        public int SectionId { get; set; }
        [ForeignKey("SectionId")]
        [ValidateNever]
        public Section Section { get; set; }
    }
}
