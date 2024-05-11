using YYBagProgram.Models.Cart;
using System.Text.Json;

namespace YYBagProgram.Service
{
    public class CartService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CartService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool isCartSessionExist()
        {
            return _contextAccessor.HttpContext.Session.Keys.Contains("Cart");
        }
        
        public Cart GetCartSession()
        {
            string cartJson = _contextAccessor.HttpContext.Session.GetString("Cart") ?? string.Empty;

            if (!string.IsNullOrEmpty(cartJson)) 
            {
                return JsonSerializer.Deserialize<Cart>(cartJson);
            }
            
            return new Cart();
        }

        public void SetCartSession(string inputJson)
        {
            CartItem cartItem = new CartItem();

            if (!string.IsNullOrEmpty(inputJson))
            {
                cartItem = JsonSerializer.Deserialize<CartItem>(inputJson);
            }

            //如果cart裡面有cartitem同樣的id_color則數量相加, 沒有的話就新增一個
            Cart cart = GetCartSession();
            int existingIndex = cart.Items.FindIndex(item => item.id_color.Equals(cartItem.id_color));
            if (existingIndex != -1)
            {
                cart.Items[existingIndex].quantity += cartItem.quantity;
            }
            else
            {
                cart.Items.Add(cartItem);
            }        
        }

        public void RemoveCartSession()
        {
            _contextAccessor.HttpContext.Session.Remove("Cart");
        }
    }
}
