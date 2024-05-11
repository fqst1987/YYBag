namespace YYBagProgram.Models.Cart
{
    [Serializable]
    public class Cart
    {
        public Cart()
        {
            this.Items = new List<CartItem>();
        }

        public List<CartItem> Items;

        public int TotalAmount
        {
            get
            {
                int totalamount = 0;
                foreach(var item in Items)
                {
                    totalamount += item.amount;
                }
                return totalamount;
            }
        }
    }
}
