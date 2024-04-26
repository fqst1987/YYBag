using System.ComponentModel;

namespace YYBagProgram.Models.ViewModel
{
    public class ProductDetailViewModel
    {
        public Product? Product { get; set; }

        public IEnumerable<ProductColor>? ProductColor { get; set; }

        public IEnumerable<ProductsColorDetail>? ProductColorsDetail { get; set; }

    }
}
