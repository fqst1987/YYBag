using Microsoft.AspNetCore.Mvc;
using YYBagProgram.Service;
using YYBagProgram.Models.CartFolder;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoCart([FromBody] CartItem postData)
        {
            await _cartService.SetCartSession(postData);

            return Json(new { success = true });
        }

        [HttpGet]
        [Route("Cart/GetCartItemCount")]
        public IActionResult GetCartItemCount()
        {
            int cartItemCount = _cartService.GetCartSession().Items.Count;
            ViewData["CartItemCount"] = cartItemCount;
            return PartialView("_CartItemCountPartial", cartItemCount);
        }

        [HttpGet]
        [Route("Cart/CartItems")]
        public IActionResult CartItems()
        {
            var cart = _cartService.GetCartSession();
            ViewData["cartItemCount"] = cart.Items.Count;
            return View(cart);
        }
    }
}
