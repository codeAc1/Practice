using Pratic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pratic.ViewModels.Home
{
    public class HomeVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> NewArrival { get; set; }
        public IEnumerable<Product> Bestseller { get; set; }
        public IEnumerable<Product> Featured { get; set; }
    }
}
