using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilibrary.Models.ViewModels
{
    public class FilterVM
    {
        [ValidateNever]
        public IEnumerable<Brand> BrandList { get; set; }

        [ValidateNever]
        public IEnumerable<Ilibrary.Models.Type> TypeList { get; set; }

        [ValidateNever]
        public IEnumerable<Product> ProductList { get; set; }

        // Preserves the search term entered by the user
        public string? SearchTerm { get; set; }
        public string? Section { get; set; }

        // Selected categories (e.g., Men, Women, Kids)
        public List<string>? SelectedCategories { get; set; }
        public List<string> SelectedBrands { get; set; }
        public List<string> SelectedAlphaSizes { get; set; } // For alphabetic sizes
        public List<string> SelectedNumericSizes { get; set; } // For numeric sizes
        public List<string>? SelectedTypes { get; set; }

        // Selected sorting option (e.g., Relevance, NameAsc, NameDesc, etc.)
        public string? SortOption { get; set; }
        public bool? Discount { get; set; }
        public bool? isAvailable { get; set; }
        public double? maxNumericSize { get; set; }
        public double? minNumericSize { get; set; }
        // Pagination properties
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

    }
}
