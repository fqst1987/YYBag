using Microsoft.AspNetCore.Mvc;

namespace YYBagProgram.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult ShowPopup()
        {
            ViewData["Message"] = "Incorrect passowrd or phone number please retry";
            return View();
        }
    }
}
