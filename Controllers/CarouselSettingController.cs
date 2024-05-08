using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.Drawing.Printing;
using System.Runtime.Intrinsics.X86;
using YYBagProgram.Data;
using YYBagProgram.Enums;
using YYBagProgram.Models;
using static System.Collections.Specialized.BitVector32;

namespace YYBagProgram.Controllers
{
    //CarouselSetting
    //[Authorize(Roles = "Administer")]
    public class CarouselSettingController : Controller
    {
        private readonly YYBagProgramContext _context;
        private readonly int pageSize = 10;
        public CarouselSettingController(YYBagProgramContext context)
        {
            _context = context;
        }

        #region 主頁
        [HttpGet]
        [Route("CarouselSetting/Main/{page}")]
        public async Task<IActionResult> Main(int page = 1)
        {
            if (_context.CarouselSetting != null)
            {
                var carouselsettings = await _context.CarouselSetting.ToListAsync();
                var totalPages = (int)Math.Ceiling(carouselsettings.Count / (double)pageSize);
                var paginatedProducts = carouselsettings.Skip((page - 1) * pageSize).Take(pageSize);

                ViewData["TotalPages"] = totalPages;
                ViewData["CurrentPage"] = page;

                if(_context.Product != null)
                {
                    ViewData["products"] = await _context.Product.ToListAsync();
                }

                return View(paginatedProducts);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 新增
        [HttpGet]
        [Route("CarouselSetting/Create/{page}")]
        public async Task<IActionResult> Create(int page)
        {
            ViewData["currentpage"] = page;

            if (_context.Product != null)
            {
                ViewData["products"] = await _context.Product.ToListAsync();
            }

            if(_context.CarouselSetting != null)
            {
                ViewData["carouselsetting"] = await _context.CarouselSetting.ToListAsync();
            }
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Route("CarouselSetting/Create/{page}")]
        public async Task<IActionResult> Create(CarouselSetting model, int page)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Main", "CarouselSetting", new {page});
            }
            else
            {
                ViewData["currentpage"] = page;
                if (_context.Product != null)
                {
                    ViewData["products"] = await _context.Product.ToListAsync();
                }
                if (_context.CarouselSetting != null)
                {
                    ViewData["carouselsetting"] = await _context.CarouselSetting.ToListAsync();
                }
                return View(model);
            }
        }
        #endregion

        #region 編輯
        [HttpGet]
        [Route("CarouselSetting/Edit/{strBagsId}/{page}")]
        public async Task<IActionResult> Edit(string strBagsId, int page)
        {
            ViewData["currentpage"] = page;
            CarouselSetting model = new CarouselSetting();

            if (_context.Product != null)
            {
                ViewData["product"] = await _context.Product.Where(row => row.strBagsId.Equals(strBagsId)).FirstOrDefaultAsync();
            }

            if (_context.CarouselSetting != null)
            {
                model = await _context.CarouselSetting.FindAsync(strBagsId) ?? model;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CarouselSetting/Edit/{strBagsId}/{page}")]
        public async Task<IActionResult> Edit(CarouselSetting model, int page)
        {
            if (ModelState.IsValid)
            {
                _context.CarouselSetting.Update(model);

                await _context.SaveChangesAsync();

                return RedirectToAction("Main", "CarouselSetting", new { page });
            }
            else
            {
                ViewData["currentpage"] = page;
                return View(model);
            }
        }
        #endregion

        #region 刪除
        [HttpGet]
        [Route("CarouselSetting/Delete/{strBagsId}/{page}")]
        public async Task<IActionResult> Delete(string strBagsId, int page)
        {
            CarouselSetting model = new CarouselSetting();

            if (_context.CarouselSetting != null)
            {
                model = await _context.CarouselSetting.FindAsync(strBagsId) ?? model;
            }

            ViewData["currentpage"] = page;
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [Route("CarouselSetting/Delete/{strBagsId}/{page}")]
        public async Task<IActionResult> DeleteComfirm(string strBagsId, int page)
        {
            var model = await _context.CarouselSetting.FindAsync(strBagsId) ?? new CarouselSetting();

            if (ModelState.IsValid)
            {
                _context.CarouselSetting.Remove(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Main", "CarouselSetting", new { page });
            }
            else
            {
                ViewData["currentpage"] = page;
                return View(model);
            }
        }
        #endregion

        [HttpGet]
        public async Task<JsonResult> GetImgUrl(string strbagsid)
        {
            string result = string.Empty;
            
            if(_context.Product != null)
            {
                result = await _context.Product.Where(row => row.strBagsId.Equals(strbagsid)).Select(row => row.strImageUrl).FirstOrDefaultAsync() ?? string.Empty;
            }

            return Json( new { strImageUrl = result });
        }
    }
}
