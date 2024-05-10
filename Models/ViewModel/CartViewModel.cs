namespace YYBagProgram.Models.ViewModel
{
    public class CartViewModel
    {
        public IList<Order> orders { get; set; }

        public IList<OrderDetail> ordersdetail { get; set; }

        public IList<Product> products { get; set; }

        public IList<ProductColor> productcolors {  get; set; }

        public IList<ProductsColorDetail> productscolordetail { get; set; }
    }
}
