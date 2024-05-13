using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YYBagProgram.Data;
using YYBagProgram.Models.ViewModel;


namespace YYBagProgram.Controllers
{
    public class HomeController : Controller
    {
        private readonly YYBagProgramContext _context;

        public HomeController(YYBagProgramContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Home()
        {
            HomeViewModel vm = new HomeViewModel();

            if(_context.CarouselSetting != null)
            {
                vm.CarouselSettings = await _context.CarouselSetting.ToListAsync();
            }
            if (_context.Product != null)
            {
                vm.Products = await _context.Product.ToListAsync();
            }
            if (_context.Classification != null)
            {
                vm.Classifications = await _context.Classification.ToListAsync();
            }
            if (_context.ProductColor != null)
            {
                vm.ProductColors = await _context.ProductColor.ToListAsync();
            }

            return View(vm);
        }
    }
}
