using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilibrary.Models
{
    public class Sizes
    {
        public int id { get; set; }
        public string? size { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("BrandId")]
        [ValidateNever]
        public Product? product { get; set; }
    }
}
