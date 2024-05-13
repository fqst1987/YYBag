namespace YYBagProgram.Models.Cart
{
    [Serializable]
    public class CartItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string colorid { get; set; }
        public string color { get; set; }
        public int status { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public string imgurl { get; set; }
        public int amount
        {
            get
            {
                return (int)(price * quantity);
            }
        }
    }
}
