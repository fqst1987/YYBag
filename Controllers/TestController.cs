using Microsoft.AspNetCore.Mvc;
using YYBagProgram.Service;

namespace YYBagProgram.Controllers
{
    public class TestController : Controller
    {
        private readonly ECPayService _payService;

        public TestController(ECPayService payService)
        {
            _payService = payService;
        }

        [HttpGet]
        public IActionResult CVSMap()
        {
            return View();
        }

    }
}
