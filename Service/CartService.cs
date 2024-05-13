using YYBagProgram.Models.Cart;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;

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
            return _contextAccessor.HttpContext.Session.Keys.Contains("cart");
        }
        
        public Cart GetCartSession()
        {
            byte[] cartBytes = _contextAccessor.HttpContext.Session.Get("cart");

            if (cartBytes != null)
            {
                string cartJson = Encoding.UTF8.GetString(cartBytes);
                return JsonSerializer.Deserialize<Cart>(cartJson);
            }

            return new Cart();
        }

        public async Task SetCartSession(string inputJson)
        {
            CartItem cartItem = new CartItem();

            if (!string.IsNullOrEmpty(inputJson))
            {
                cartItem = JsonSerializer.Deserialize<CartItem>(inputJson);
            }

            //如果cart裡面有cartitem同樣的id_color則數量相加, 沒有的話就新增一個
            Cart cart = GetCartSession();
            int existingIndex = cart.Items.FindIndex(item => item.colorid.Equals(cartItem.colorid));
            if (existingIndex != -1)
            {
                cart.Items[existingIndex].quantity += cartItem.quantity;
            }
            else
            {
                cart.Items.Add(cartItem);
            }
            _contextAccessor.HttpContext.Session.Set("cart", SerializeObject(cart));

            await Task.CompletedTask;
        }

        public async Task SetCartSession(CartItem cartItem)
        {
            //如果cart裡面有cartitem同樣的id_color則數量相加, 沒有的話就新增一個
            Cart cart = GetCartSession();
            int existingIndex = cart.Items.FindIndex(item => item.colorid.Equals(cartItem.colorid));
            if (existingIndex != -1)
            {
                cart.Items[existingIndex].quantity += cartItem.quantity;
            }
            else
            {
                cart.Items.Add(cartItem);
            }

            _contextAccessor.HttpContext.Session.Set("cart", SerializeObject(cart));

            await Task.CompletedTask;
        }

        private byte[] SerializeObject(object obj)
        {
            var jsonString = string.Empty;
            if (obj == null)
                return null;

            jsonString = JsonSerializer.Serialize(obj);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        private T DeserializeObject<T>(byte[] data)
        {
            if (data == null)
                return default;

            var jsonString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(jsonString);
        }

        public void RemoveCartSession()
        {
            _contextAccessor.HttpContext.Session.Remove("Cart");
        }
    }
}
