using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YYBagProgram.Controllers
{
    //Cart
    public class CartController : Controller
    {
        //[Authorize]
        public IActionResult CartMain()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddtoCart(string strBagsId)
        {
            return View();
        }
    }
}
