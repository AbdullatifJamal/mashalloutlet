using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ilibrary.Models.ViewModels
{
    public class DetailsVM
    {
        public Product product { get; set; }
        public IEnumerable<Product> Products { get; set; }

    }
}
