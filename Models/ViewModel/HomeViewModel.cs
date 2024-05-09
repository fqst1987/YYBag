using YYBagProgram.Models;

namespace YYBagProgram.Models.ViewModel
{
    public class HomeViewModel
    {
        public IList<CarouselSetting> CarouselSettings { get; set; }

        public IList<Product> Products { get; set; }

        public IList<Classification> Classifications { get; set; }

        public IList<ProductColor> ProductColors { get; set; }
    }
}
