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
using YYBagProgram.Comm;
using YYBagProgram.Data;
using YYBagProgram.Enums;
using YYBagProgram.Models;
using static YYBagProgram.Enums.AllEnums;

namespace YYBagProgram.Controllers
{
    //ProductsColorDetails
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
        [Route("ProductColorCreate/{strBagsId}/{page}")]
        public async Task<IActionResult> ProductColorCreate(string strBagsId, int page)
        {
            ViewData["strBagsId"] = strBagsId ?? string.Empty;
            ViewData["currentpage"] = page;
            if (_context.ProductColor != null)
            {
                ViewData["productcolors"] = await _context.ProductColor.ToListAsync();
            }
            return View();
        }

        [HttpPost, ActionName("ProductColorCreate")]
        [ValidateAntiForgeryToken]
        [Route("ProductColorCreate/{strBagsId}/{page}")]
        public async Task<IActionResult> ProductColorCreate(ProductColor productColor, int page, string strBagsId)
        {
            productColor.strID = GetProductsColorNumber(productColor.strBagsId);
            if (ModelState.IsValid)
            {
                _context.ProductColor.Add(productColor);
                await _context.SaveChangesAsync();

                return RedirectToAction("ProductColorMain", "ProductsColorDetails", new { strBagsId = productColor.strBagsId, page = page });
            }

            ViewData["strBagsId"] = strBagsId ?? string.Empty;
            ViewData["currentpage"] = page;
            return View(productColor);
        }
        #endregion

        #region 商品顏色 刪除 Administer
        [HttpGet]
        [Route("DeleteMain/{strID}/{page}")]
        public async Task<IActionResult> DeleteMain(string strID, int page)
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

            ViewData["currentpage"] = page;

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
        [Route("ProductColorDetailMain/{strID}/{page}")]
        public async Task<IActionResult> ProductColorDetailMain(string strID, int page)
        {
            if (strID.Length > 0)
            {
                ProductColor proudctcolor = await _context.ProductColor.Where(row => row.strID.Equals(strID)).FirstOrDefaultAsync() ?? new ProductColor();
                ViewData["proudctcolor"] = proudctcolor;
            }
            ViewData["currentpage"] = page;

            return _context.ProductsColorDetail != null ?
                        View(await _context.ProductsColorDetail.Where(row => row.strID.Equals(strID)).ToListAsync()) :
                        NotFound();
        }

        #endregion

        #region 商品顏色數量 新增 Administer
        [HttpGet]
        [Route("Create/{strID}/{strColor}/{page}")]
        public async Task<IActionResult> Create(string strColor, string strID, int page)
        {
            ProductsColorDetail productsColorDetail = new ProductsColorDetail();
            productsColorDetail.strID = strID;
            productsColorDetail.strColor = strColor;
            productsColorDetail.Images = string.Empty;

            string strBagsId = string.Empty;
            string imgurls = string.Empty;
            //照片選項
            if (_context?.Product != null)
            {
                strBagsId = await _context.ProductColor.Where(row => row.strID.Equals(strID)).Select(row => row.strBagsId).FirstOrDefaultAsync() ?? string.Empty;
                imgurls = await _context.Product.Where(row => row.strBagsId.Equals(strBagsId)).Select(row => row.strImageUrl).FirstOrDefaultAsync() ?? string.Empty;
            }
            ViewData["imgurls"] = imgurls;

            ViewData["currentpage"] = page;

            if(_context?.ProductsColorDetail != null)
            {
                ViewData["productcolordetail"] = await _context.ProductsColorDetail.Where(row => row.strColor.Equals(strColor) && row.strID.Equals(strID)).ToListAsync();
            }


            return View(productsColorDetail);
        }


        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Route("Create/{strID}/{strColor}/{page}")]
        public async Task<IActionResult> Create(ProductsColorDetail productsColorDetail, IFormFileCollection imagefiles, int page)
        {
            string imgpath = string.Empty;
            string strFolderName = "bagimgs";
            if (imagefiles != null && imagefiles.Count > 0)
            {
                imgpath = Methods.UploadImg(_enviroment, imagefiles, strFolderName);
                productsColorDetail.Images = imgpath;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(productsColorDetail);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }


                return RedirectToAction("ProductColorDetailMain", "ProductsColorDetails", new { productsColorDetail.strID, page });
            }
            else
            {
                string strBagsId = string.Empty;
                string imgurls = string.Empty;
                //照片選項
                if (_context?.Product != null)
                {
                    strBagsId = await _context.ProductColor.Where(row => row.strID.Equals(productsColorDetail.strID)).Select(row => row.strBagsId).FirstOrDefaultAsync() ?? string.Empty;
                    imgurls = await _context.Product.Where(row => row.strBagsId.Equals(strBagsId)).Select(row => row.strImageUrl).FirstOrDefaultAsync() ?? string.Empty;
                }
                ViewData["imgurls"] = imgurls;
                ViewData["currentpage"] = page;
                if (_context.ProductsColorDetail != null)
                {
                    ViewData["productcolordetail"] = await _context.ProductsColorDetail.Where(row => row.strColor.Equals(productsColorDetail.strColor) && row.strID.Equals(productsColorDetail.strID)).ToListAsync();
                }
                return View(productsColorDetail);
            }
        }
        #endregion

        #region 商品顏色數量 編輯 Administer
        [HttpGet]
        [Route("Edit/{strID}/{strColor}/{iProductStatus}/{page}")]
        public async Task<IActionResult> Edit(string strID, string strColor, int iProductStatus, int page)
        {
            string strBagsId = string.Empty;
            string imgurls = string.Empty;
            //照片選項
            if (_context?.Product != null)
            {
                strBagsId = await _context.ProductColor.Where(row => row.strID.Equals(strID)).Select(row => row.strBagsId).FirstOrDefaultAsync() ?? string.Empty;
                imgurls = await _context.Product.Where(row => row.strBagsId.Equals(strBagsId)).Select(row => row.strImageUrl).FirstOrDefaultAsync() ?? string.Empty;
            }

            if (strID == null || _context.ProductsColorDetail == null )
            {
                return NotFound();
            }

            var productsColorDetail = await _context.ProductsColorDetail
            .Where(row => row.strID.Equals(strID) && row.strColor.Equals(strColor) && (int)row.ProductStatus == iProductStatus).FirstOrDefaultAsync();
            ViewData["productcolordetail"] = productsColorDetail;
            ViewData["imgurls"] = imgurls;
            ViewData["currentpage"] = page;

            return View(productsColorDetail);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Route("Edit/{strID}/{strColor}/{iProductStatus}/{page}")]
        public async Task<IActionResult> Edit(ProductsColorDetail model, IFormFileCollection imagefiles, int page)
        {

            string imgpath = string.Empty;
            string strFolderName = "bagimgs";
            if (imagefiles != null && imagefiles.Count > 0)
            {
                imgpath = Methods.UploadImg(_enviroment, imagefiles, strFolderName, model.Images);
                model.Images = imgpath;
            }

            if (ModelState.IsValid)
            {          
                try
                {
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@strID", model.strID),
                        new SqlParameter("@strColor", model.strColor),
                        new SqlParameter("@iProductStatus", (int)model.ProductStatus),
                        new SqlParameter("@iTotal", model.iTotal),
                        new SqlParameter("@iRemain", model.iRemain),
                        new SqlParameter("@iPrice", model.iPrice),
                        new SqlParameter("@Images", model.Images),
                        new SqlParameter("@iDeliveryDays", model.iDeliveryDays),
                        new SqlParameter("@isOnline", model.isOnline)
                    };

                    string sql = @"
UPDATE ProductsColorDetail 
SET iTotal = @iTotal, iRemain = @iRemain, iPrice = @iPrice, Images = @Images, iDeliveryDays = @iDeliveryDays, isOnline = @isOnline 
WHERE strId = @strID AND strColor = @strColor AND ProductStatus = @iProductStatus";
                    await _context.Database.ExecuteSqlRawAsync(sql, sqlParameters);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Content(ex.Message);
                }

                return RedirectToAction("ProductColorDetailMain", "ProductsColorDetails", new { model.strID, page});
            }
            else
            {
                string strBagsId = string.Empty;
                string imgurls = string.Empty;
                //照片選項
                if (_context?.Product != null)
                {
                    strBagsId = await _context.ProductColor.Where(row => row.strID.Equals(model.strID)).Select(row => row.strBagsId).FirstOrDefaultAsync() ?? string.Empty;
                    imgurls = await _context.Product.Where(row => row.strBagsId.Equals(strBagsId)).Select(row => row.strImageUrl).FirstOrDefaultAsync() ?? string.Empty;
                }
                var productsColorDetail = await _context.ProductsColorDetail
           .Where(row => row.strID.Equals(model.strID) && row.strColor.Equals(model.strColor) && (int)row.ProductStatus == (int)model.ProductStatus).FirstOrDefaultAsync();
                ViewData["productcolordetail"] = productsColorDetail;
                ViewData["currentpage"] = page;
                ViewData["imgurls"] = imgurls;
                return View(model);
            }
        }
        #endregion

        #region 商品顏色數量 刪除 Administer
        [HttpGet]
        [Route("Delete/{strID}/{strColor}/{ProductStatus}/{page}")]
        public async Task<IActionResult> Delete(string strID, string strColor, AllEnums.ProductStatus ProductStatus, int page)
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
            ViewData["currentpage"] = page;

            return View(productsColorDetail);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{strID}/{strColor}/{ProductStatus}/{page}")]
        public async Task<IActionResult> DeleteConfirmed(string strID, string strColor, AllEnums.ProductStatus ProductStatus, int page)
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
            return RedirectToAction("ProductColorDetailMain", "ProductsColorDetails", new {strID, page});
        }
        #endregion


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
    }
}
