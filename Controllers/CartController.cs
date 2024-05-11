using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YYBagProgram.Service;
using YYBagProgram.Models.Cart;

namespace YYBagProgram.Controllers
{
    //Cart
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        //[Authorize]
        public IActionResult CartMain()
        {
            return View();
        }

        [HttpPost]
        [Route("Cart/AddtoCart")]
        public async Task<IActionResult> AddtoCart([FromBody] CartItem postData)
        {
            return Json(new { success = true });
        }
    }
}
