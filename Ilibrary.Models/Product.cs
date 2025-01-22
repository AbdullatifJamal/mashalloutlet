using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilibrary.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Size { get; set; }
        public int? Discount { get; set; }
        public DateTime? UploadDate { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? ManySizes { get; set; }
        public bool? OnBanner { get; set; }
        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        [ValidateNever]
        public Brand brand { get; set; }
        
        [Required]
        
        public double Price { get; set; }
        [Required]

        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        [ValidateNever]
        public Type Type { get; set; }
        [ValidateNever]

        public byte[]? MainImage { get; set; }
        public byte[][]? SecondaryImages { get; set; }

    }
}
