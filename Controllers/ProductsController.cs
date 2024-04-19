using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using YYBagProgram.Data;
using YYBagProgram.Models;

namespace YYBagProgram.Controllers
{
    public class ProductsController : Controller
    {
        private readonly YYBagProgramContext _context;

        public ProductsController(YYBagProgramContext context)
        {
            _context = context;
        }

        #region 商品 主頁 Administer 
        //Administer
        [HttpGet]
        [Route("Products/Main")]
        //[Authorize(Roles = "Administer")]
        public async Task<IActionResult> Main()
        {
              return _context.Product != null ? 
                          View(await _context.Product.ToListAsync()) :
                          Problem("Entity set 'YYBagProgramContext.Product'  is null.");
        }
        #endregion

        #region 商品 主頁 Client
        //Client
        //用AllProductsViewModel顯示
        [HttpGet]
        [Route("AllProducts")]
        public async Task<IActionResult> AllProducts()
        {
            //本月主打 todo
           
            //全部商品 todo

            return View();
        }
        #endregion

        #region 商品 詳細內容 Administer Client
        //Client
        [HttpGet]
        [Route("Products/Details/{strBagsId}")]
        public async Task<IActionResult> Details(string? strBagsId)
        {
            //建立一個vm
            ProductDetailViewModel vm = new ProductDetailViewModel();

            //商品主頁(單一)
            if (_context.Product.Any(row => row.strBagsId.Equals(strBagsId)))
            {
                var product = await _context.Product.FindAsync(strBagsId);
                vm.Product = product;
            }

            //商品顏色主頁(多個)
            if(_context.ProductColor.Any(row => row.strBagsId.Equals(strBagsId)))
            {
                var productcolor = await _context.ProductColor.Where(row => row.strBagsId.Equals(strBagsId)).ToListAsync();

                vm.ProductColor = productcolor;
            }

            //商品顏色數量細項
            if (vm.ProductColor != null)
            {
                List<ProductsColorDetail> productcolordetail = new List<ProductsColorDetail>();
                
                foreach(var obj in vm.ProductColor)
                {
                    var productcolordetail_1 = await _context.ProductsColorDetail.Where(row => row.strID.Equals(obj.strID)).ToListAsync();
                    foreach(var obj2 in productcolordetail_1)
                    {
                        productcolordetail.Add(obj2);
                    }
                }
                vm.ProductColorsDetail = productcolordetail;
            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetData(string strID, string strColor)
        {
            var data = _context.ProductsColorDetail.Where(row => row.strID.Equals(strID) && row.strColor.Equals(strColor)).ToList();
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetDataRemain(string strID, string strColor, int ProductStatus)
        {
            var data = _context.ProductsColorDetail.Where(row => row.strID.Equals(strID) && row.strColor.Equals(strColor) && (int)row.ProductStatus == ProductStatus).ToList();
            return Json(data);
        }

        #endregion

        #region 商品 新增 Administer
        [HttpGet]
        [Route("Products/Create")]
        [Authorize(Roles = "Administer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Products/Create")]
        [Authorize(Roles = "Administer")]
        public async Task<IActionResult> Create(Product product)
        {
            product.strBagsId = GetBagID();

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Main));
            }

            return View(product);
        }
        #endregion

        #region 商品 編輯 Administer
        [HttpGet]
        [Route("Edit/{strBagsId}")]
        public async Task<IActionResult> Edit(string strBagsId)
        {
            if (strBagsId == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(strBagsId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Route("Edit/{strBagsId}")]
        public async Task<IActionResult> Edit(Product product)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    if (!ProductExists(product.strBagsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Main", "Products");
            }
            return View(product);
        }
        #endregion

        #region 商品 刪除 Administer
        [HttpGet]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Administer")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.strBagsId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string strBagsId)
        {
         
            if (_context.Product == null)
            {
                return Problem("Entity set 'YYBagProgramContext.Product'  is null.");
            }
            //刪除商品
            var product = await _context.Product.FindAsync(strBagsId);
            if (product != null)
            {           
                _context.Product.Remove(product);
            }
            //刪除顏色、數量
            var productColor = await _context.ProductColor.FindAsync(strBagsId);
            if(productColor != null) 
            {
                string strID = productColor.strID;

                //刪除商品顏色表
                SqlParameter p1 = new SqlParameter("@iBagsId", strBagsId);
                string sql = "DELETE ProductColor WHERE iBagsId = @iBagsId";
                _context.Database.ExecuteSqlRawAsync(sql, p1);

                //刪除商品顏色明細
                var productsColorDetails = await _context.ProductsColorDetail.FindAsync(strID);
                if (productsColorDetails != null)
                {
                    p1 = new SqlParameter("@strID", strID);
                    sql = "DELETE ProductsColorDetail WHERE strID = @strID";
                    _context.Database.ExecuteSqlRawAsync(sql, p1);
                }
            }
            
            await _context.SaveChangesAsync();
            if (User.IsInRole("Administer"))
            {
                return RedirectToAction("Main", "Products");
            }
            else
            {
                return RedirectToAction("AllProducts", "Products");
            }

        }
        #endregion

        private bool ProductExists(string id)
        {
          return (_context.Product?.Any(e => e.strBagsId == id)).GetValueOrDefault();
        }

        //根據創建的年月 + 序號
        private string GetBagID()
        {
            string result = string.Empty;

            //年月
            string strYearMonth = DateTime.Now.ToString("yyyyMM");

            //Filter
            string strFilter = "1 = 1";

            SqlParameter p1 = new SqlParameter("@YearMonth", strYearMonth);

            var sql = @"SELECT * FROM Product WHERE SUBSTRING(strBagsId, 1, 6) = @YearMonth";

            var data = _context.Product.FromSqlRaw(sql, p1).ToList();

            if (data.Count == 0)
            {
                result = strYearMonth + "01";
            }
            else
            {

                string temp = (Int16.Parse(data.Max(row => row.strBagsId.Substring(row.strBagsId.Length - 2, 2))) + 1).ToString("D2");
                result = strYearMonth + temp;
            }

            return result;
        }
    }
}
