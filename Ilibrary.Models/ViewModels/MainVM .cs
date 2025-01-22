using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilibrary.Models.ViewModels
{
    public class MainVM
    {
        [ValidateNever]
        public IEnumerable<Product> onBannerProducts { get; set; }
        [ValidateNever]
        public IEnumerable<Product> onSaleProducts { get; set; }
        [ValidateNever]
        public IEnumerable<Product> LatestProducts { get; set; }
    }
}
