﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YYBagProgram.Data;
using YYBagProgram.Enums;
using YYBagProgram.Models;

namespace YYBagProgram.Controllers
{
    
    public class MonthlyHotController : Controller
    {
        private readonly YYBagProgramContext _context;
        private readonly IWebHostEnvironment _enviroment;
        public MonthlyHotController(YYBagProgramContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _enviroment = enviroment;
        }

        #region 主頁
        [HttpGet]
        //[Authorize(Roles = "Administer")]
        public async Task<IActionResult> Main()
        {
            return _context.MonthlyHots != null ?
                        View(await _context.MonthlyHots.ToListAsync()) :
                        Problem("Entity set 'YYBagProgramContext.MonthlyHot' is null.");
        }

        [HttpGet]
        public async Task<IActionResult> MonthlyHotMain()
        {
            //先找出全部的MonthlyHot
            if (_context.MonthlyHots == null)
            {
                return NotFound();
            }

            if (_context.Product!= null)
            {
                return NotFound();
            }

            MonthlyHotViewModel vm = new MonthlyHotViewModel();
            vm.monthlyHot = await _context.MonthlyHots.ToListAsync();

            return View(vm);
        }
        #endregion

        #region 新增
        //[Authorize(Roles = "Administer")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            MonthlyHot monthlyHot = new MonthlyHot();
            monthlyHot.Year = DateTime.Now.Year;
            monthlyHot.Month = DateTime.Now.Month;

            if (_context.Product != null)
            {
                ViewData["product"] = await _context.Product.ToListAsync();
            }

            return View(monthlyHot);
        }
        //[Authorize(Roles = "Administer")]
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonthlyHot model, IFormFile imagefiles)
        {
            string imgpath = string.Empty;
            if (!UploadImg(imagefiles, out imgpath))
            {
                throw new Exception("上傳圖片檔案失敗");
            }
            model.img = imgpath;

            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Main));
            }
            return View(model);
        }
        #endregion

        #region 編輯
        [HttpGet]
        [Route("MonthlyHot/Edit/{year}/{month}/{strBagsId}")]
        //[Authorize(Roles = "Administer")]
        public async Task<IActionResult> Edit(int year, int month, string strbagsid)
        {
            var modeltemp = await _context.MonthlyHots.FindAsync(year, month, strbagsid);
            if (modeltemp == null)
            {
                return NotFound();
            }
            var model = await _context.MonthlyHots.Where(row => row.Year == year && row.Month == month && row.strBagsId.Equals(strbagsid)).FirstOrDefaultAsync();
            if (model == null)
            {
                return NotFound();
            }

            MonthlyHotViewModel vm = new MonthlyHotViewModel();
            
            if (_context.Product != null)
            {
                vm.product = await  _context.Product.ToListAsync();
            }

            vm.monthlyHot = model;
            vm.OriImages = model.img;

            return View(vm);

        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Route("MonthlyHot/Edit/{Year}/{Month}/{strBagsId}")]
        public async Task<IActionResult> Edit(MonthlyHotViewModel vm, IFormFile imagefiles, string OriImages)
        {
            string imgpath = string.Empty;
            MonthlyHot monthlyhot = new MonthlyHot();

            if (ModelState.IsValid)
            {
                monthlyhot = vm.monthlyHot;

                if (imagefiles != null)
                {
                    //刪除原本舊的檔案
                    System.IO.File.Delete(OriImages);

                    if (UploadImg(imagefiles, out imgpath))
                    {
                        monthlyhot.img = imgpath;
                    }

                }
                else
                {
                    monthlyhot.img = vm.OriImages;
                }

                try
                {
                    _context.Update(monthlyhot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Main", "MonthlyHot");
            }

            return View(vm);
        }
        #endregion

        #region 刪除
        //[Authorize(Roles = "Administer")]
        [HttpGet]
        [Route("Delete/{Year}/{Month}/{strBagsId}")]
        public async Task<IActionResult> Delete(int year, int month, string strBagsId)
        {
            var modelTemp = _context.MonthlyHots.FindAsync(year, month, strBagsId);
            if (modelTemp == null)
            {
                return NotFound();
            }
            var model = await _context.MonthlyHots.Where(row => row.Year == year && row.Month == month && row.strBagsId.Equals(strBagsId)).FirstAsync();
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //[Authorize(Roles = "Administer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{Year}/{Month}/{strBagsId}")]
        public async Task<IActionResult> DeleteConfirmed(int year, int month, string strBagsId)
        {
            if (_context.MonthlyHots == null)
            {
                return NotFound();
            }
            var model = await _context.MonthlyHots.Where(row => row.Year == year && row.Month == month && row.strBagsId.Equals(strBagsId)).FirstAsync();
            if (model != null)
            {
                _context.MonthlyHots.Remove(model);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Main", "MonthlyHot");
        }
        #endregion

        private bool UploadImg(IFormFile target, out string targetpath)
        {
            if (target != null && target.Length > 0)
            {
                var uploadFolder = Path.Combine(_enviroment.WebRootPath, "upload", "monthlyhot", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("D2"));
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                string strFileName = Guid.NewGuid().ToString() + "_" + target.FileName;
                targetpath = Path.Combine(uploadFolder, strFileName);

                using (var fileStream = new FileStream(targetpath, FileMode.Create))
                {
                    target.CopyTo(fileStream);
                }

                return true;
            }
            else
            {
                targetpath = string.Empty;
                return false;
            }
        }
    }
}
