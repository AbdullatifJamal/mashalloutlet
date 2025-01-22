using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilibrary.Models.ViewModels
{
    public class TypeVM
    {
        public Type Type { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SectionList { get; set; }
    }
}
