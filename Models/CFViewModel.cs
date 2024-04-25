namespace YYBagProgram.Models
{
    public class CFViewModel
    {
        public IList<Product>? Products { get; set; }

        public Classification? Classification { get; set; }

        public IList<ClassificationDetail>? ClassificationDetails { get; set; }

        public IList<ProductsColorDetail>? ProductsColorDetails { get; set; }

        public IList<ProductColor>? ProductColors { get; set; }

        //下面兩個都會出現在每個畫面
        public IList<CarouselSetting>? CarouselSettings { get; set; }

        public IList<Classification>? Classfications { get; set; }
    }
}
