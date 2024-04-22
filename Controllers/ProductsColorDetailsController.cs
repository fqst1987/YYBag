using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using YYBagProgram.Data;
using YYBagProgram.Enums;
using YYBagProgram.Models;

namespace YYBagProgram.Controllers
{
    [Route("ProductsColorDetails")]
    //[Authorize(Roles = "Administer")]
    public class ProductsColorDetailsController : Controller
    {
        private readonly YYBagProgramContext _context;
        private readonly IWebHostEnvironment _enviroment;

        public ProductsColorDetailsController(YYBagProgramContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _enviroment = enviroment;
        }

        #region 商品顏色 主頁 Administer
        [HttpGet]
        [Route("ProductColorMain/{strBagsId}/{page}")]
        public async Task<IActionResult> ProductColorMain(string strBagsId, int page)
        {
            ViewData["strProductName"] = _context.Product.Where(row => row.strBagsId.Equals(strBagsId)).FirstOrDefault()?.strBagsName;
            ViewData["strBagsId"] = strBagsId;
            ViewData["currentpage"] = page;
            return View(await _context.ProductColor.Where(row => row.strBagsId.Equals(strBagsId)).ToListAsync());
        }
        #endregion

        #region 商品顏色 新增 Administer
        [HttpGet]
        [Route("ProductColorCreate/{strBagsId}")]
        public IActionResult ProductColorCreate(string strBagsId)
        {
            ViewData["strBagsId"] = strBagsId ?? string.Empty;
            return View();
        }

        [HttpPost, ActionName("ProductColorCreate")]
        [ValidateAntiForgeryToken]
        [Route("ProductColorCreate/{strBagsId}")]
        public async Task<IActionResult> ProductColorCreate(ProductColor productColor)
        {
            productColor.strID = GetProductsColorNumber(productColor.strBagsId);
            if (ModelState.IsValid)
            {
                _context.ProductColor.Add(productColor);
                await _context.SaveChangesAsync();

                return RedirectToAction("ProductColorMain", "ProductsColorDetails", new { strBagsId = productColor.strBagsId });
            }
            return View(productColor);
        }
        #endregion

        #region 商品顏色 刪除 Administer
        [HttpGet]
        [Route("DeleteMain/{strID}")]
        public async Task<IActionResult> DeleteMain(string strID)
        {

            if (!_context.ProductColor.Any(row => row.strID.Equals(strID)) || strID == null)
            {
                return NotFound();
            }

            var productColor = await _context.ProductColor.Where(row => row.strID.Equals(strID)).FirstOrDefaultAsync();
            var productsColorDetail = await _context.ProductsColorDetail.Where(row => row.strID.Equals(strID)).ToListAsync();

            ProductColorViewModel productColorViewModel = new ProductColorViewModel();
            productColorViewModel.ProductColor = productColor;
            productColorViewModel.ProductsColorDetail = productsColorDetail;

            return View(productColorViewModel);
        }

        [HttpPost]
        [Route("DeleteMain/{strID}")]
        public async Task<IActionResult> DeleteMainConfirm(string strID)
        {
            
            if (strID == null)
            {
                return NotFound();
            }

            if (_context.ProductColor.Where(row => row.strID.Equals(strID)).FirstOrDefault() != null)
            {
                // 刪除顏色、數量
                SqlParameter p1 = new SqlParameter("@strID", strID);
                string sql = @"DELETE ProductColor WHERE strID = @strID";
                _context.Database.ExecuteSqlRawAsync(sql, p1);
            }

            if (_context.ProductsColorDetail.Where(row => row.strID.Equals(strID)).FirstOrDefault() != null)
            {
                //刪除商品顏色明細
                SqlParameter p1 = new SqlParameter("@strID", strID);
                string sql = "DELETE ProductsColorDetail WHERE strID = @strID";
                _context.Database.ExecuteSqlRawAsync(sql, p1);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Main", "Products");
        }
        #endregion

        #region 商品顏色數量 主頁 Administer
        [HttpGet]
        [Route("Main/{strID}")]
        public async Task<IActionResult> Main(string strID)
        {
            if (strID.Length > 0)
            {
                ProductColor proudctcolor = await _context.ProductColor.Where(row => row.strID.Equals(strID)).FirstOrDefaultAsync() ?? new ProductColor();
                ViewData["proudctcolor"] = proudctcolor;
            }

            return _context.ProductsColorDetail != null ?
                        View(await _context.ProductsColorDetail.Where(row => row.strID.Equals(strID)).ToListAsync()) :
                        NotFound();
        }

        #endregion

        #region 商品顏色數量 新增 Administer
        [HttpGet]
        [Route("Create/{strID}/{strColor}")]
        public IActionResult Create(string strColor, string strID)
        {
            ProductsColorDetail productsColorDetail = new ProductsColorDetail();
            productsColorDetail.strID = strID;
            productsColorDetail.strColor = strColor;
            productsColorDetail.Images = string.Empty;
            //找總共進了多少個iTotal
            //找ProductColorDetail裡剩幾個
            //找賣出了幾個
            return View(productsColorDetail);
        }


        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Route("Create/{strID}/{strColor}")]
        public async Task<IActionResult> Create(ProductsColorDetail productsColorDetail, IFormFileCollection imagefiles)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    productsColorDetail.Images = GetImagesURL(imagefiles);
                    _context.Add(productsColorDetail);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }


                return RedirectToAction("Main", "ProductsColorDetails", new { strID = productsColorDetail.strID });
            }
            return View(productsColorDetail);
        }
        #endregion

        #region 商品顏色數量 編輯 Administer
        [HttpGet]
        [Route("Edit/{strID}/{strColor}/{iProductStatus}")]
        public async Task<IActionResult> Edit(string strID, string strColor, int iProductStatus)
        {
            if (strID == null || _context.ProductsColorDetail == null )
            {
                return NotFound();
            }

            var productsColorDetail = await _context.ProductsColorDetail
                .Where(row => row.strID.Equals(strID) && row.strColor.Equals(strColor) && (int)row.ProductStatus == iProductStatus).FirstOrDefaultAsync();

            if (productsColorDetail == null)
            {
                productsColorDetail.Images = string.Empty;
                return NotFound();
            }

            PrdosuctsColorDetailViewModel vm = new PrdosuctsColorDetailViewModel();
            vm.productsColorDetail = productsColorDetail;
            vm.OriImages = productsColorDetail.Images;

            return View(vm);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Route("Edit/{strID}/{strColor}/{iProductStatus}")]
        public async Task<IActionResult> Edit(PrdosuctsColorDetailViewModel vm, IFormFileCollection imagefiles)
        {
            ProductsColorDetail productsColorDetail = new ProductsColorDetail();

            if (ModelState.IsValid)
            {

                productsColorDetail = vm.productsColorDetail;
                
                //如果有上傳 就用舊的
                if (imagefiles != null && imagefiles.Any())
                {
                    productsColorDetail.Images = GetImagesURL(imagefiles);
                }
                else
                {
                    productsColorDetail.Images = vm.OriImages;                }

                try
                {
                    _context.Update(productsColorDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Main", "ProductsColorDetails", new { id = productsColorDetail.strID});
            }
            return View(vm);
        }
        #endregion

        #region 商品顏色數量 刪除 Administer
        [HttpGet]
        [Route("Delete/{strID}/{strColor}/{ProductStatus}")]
        public async Task<IActionResult> Delete(string strID, string strColor, AllEnums.ProductStatus ProductStatus)
        {
            if (strID == null || _context.ProductsColorDetail == null)
            {
                return NotFound();
            }

            int iProductStatus = (int)ProductStatus;

            var productsColorDetail = await _context.ProductsColorDetail
                .FirstOrDefaultAsync(row => (int)row.ProductStatus == iProductStatus && row.strColor.Equals(strColor) && row.strID.Equals(strID));

            if (productsColorDetail == null)
            {
                return NotFound();
            }

            return View(productsColorDetail);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{strID}/{strColor}/{ProductStatus}")]
        public async Task<IActionResult> DeleteConfirmed(string strID, string strColor, AllEnums.ProductStatus ProductStatus)
        {
            if (_context.ProductsColorDetail == null)
            {
                return NotFound();
            }
            int iProductStatus = (int) ProductStatus;
            var productsColorDetail = _context.ProductsColorDetail.Where(row => row.strID.Equals(strID) && row.strColor.Equals(strColor) && (int)row.ProductStatus == iProductStatus).FirstOrDefault();

            if (productsColorDetail != null)
            {
                _context.ProductsColorDetail.Remove(productsColorDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Main", "ProductsColorDetails", new { strID = strID});
        }
        #endregion


        private string GetImagesURL(IFormFileCollection imagefiles)
        {
            List<string> listImgFileName = new List<string>();
            string strImageURLs = string.Empty;
            if (imagefiles != null && imagefiles.Count > 0)
            {
                if (ValidateFile(imagefiles))
                {
                    foreach (IFormFile file in imagefiles)
                    {
                        listImgFileName.Add(file.FileName);

                    }
                    strImageURLs = String.Join(";", listImgFileName);
                }
            }
            return strImageURLs;
        }

        private string GetProductsColorNumber(string id)
        {
            string result = string.Empty;
            var data = _context.ProductColor;
            
            if (data != null)
            {
                if (data.Any(row => row.strBagsId.Equals(id)))
                {
                    string temp = (Int16.Parse(data.Where(row => row.strBagsId.Equals(id)).Max(row => row.strID.Substring(row.strID.Length - 2, 2))) + 1).ToString("D2");

                    result = id.Substring(0, 8) + temp;
                }
                else
                {
                    result = id + "01";
                }
            }

            return result;
        }

        private bool ValidateFile(IFormFileCollection files)
        {
            foreach (IFormFile file in files)
            {
                if (file.Length < 1024 * 1024 && file.ContentType.StartsWith("image/"))
                    continue;
                else
                    return false;
            }
            return true;
        }

        private string SaveImage(IFormFile file)
        {
            string filePath = Path.Combine(_enviroment.ContentRootPath, @"wwwroot\images\bagimages");
            string fileName = Path.GetFileNameWithoutExtension(file.FileName) + Path.GetExtension(file.FileName);
            string fileFullPath = Path.Combine(filePath, fileName);

            return fileFullPath;
        }
    }
}
