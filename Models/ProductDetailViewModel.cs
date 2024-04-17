using System.ComponentModel;

namespace YYBagProgram.Models
{
    public class ProductDetailViewModel
    {
        public Product? Product { get; set; }

        public IEnumerable<ProductColor>? ProductColor { get; set; }

        public IEnumerable<ProductsColorDetail>? ProductColorsDetail { get; set; }

    }
}
