using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Drawing.Printing;
using YYBagProgram.Data;
using YYBagProgram.Models;
using YYBagProgram.Models.ViewModel;

namespace YYBagProgram.Controllers
{
    //[Authorize(Roles = "Administer")]
    //Classification
    public class ClassificationController : Controller
    {
        private readonly YYBagProgramContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;
 
        public ClassificationController(YYBagProgramContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        #region 自訂分類 主頁
        [HttpGet]
        [Route("Classification/Main/{page}")]
        public async Task<IActionResult> Main(int page)
        {
            int pageSize = 10;
            if (_context.Classification != null)
            {
                var classifications = await _context.Classification.ToListAsync();
                var totalPages = (int)Math.Ceiling(classifications.Count / (double)pageSize);
                var paginatedProducts = classifications.Skip((page - 1) * pageSize).Take(pageSize);

                ViewData["TotalPages"] = totalPages;
                ViewData["CurrentPage"] = page;

                return View(paginatedProducts);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("Classification/ClassificationMain/{Id}/{page}")]
        public async Task<IActionResult> ClassificationMain(string Id, int page)
        {
            int pageSize = 3;
            ViewData["id"] = Id;

            CFViewModel vm = new CFViewModel();
            List<string> listBagsId = new List<string>();
            List<string> liststrID = new List<string>();

            //找到分類
            if (_context.Classification != null && Id != null)
            {
                vm.Classfications = await _context.Classification.ToListAsync();
                vm.Classification = await _context.Classification.Where(row => row.Id.Equals(Id)).FirstOrDefaultAsync();
                ViewData["titlemsg"] = vm.Classification?.Name;
            }
            //找到分類明細
            if (_context.ClassificationDetail != null)
            {
                vm.ClassificationDetails = await _context.ClassificationDetail.Where(row => row.Id.Equals(Id)).ToListAsync();
                //找到分類明細中的商品項目
                listBagsId = vm.ClassificationDetails.Select(row => row.strBagsId).Distinct().ToList();
                //看會有幾頁
                var totalPages = (int)Math.Ceiling(listBagsId.Count / (double)pageSize);
                var paginatedProducts = listBagsId.Skip((page - 1) * pageSize).Take(pageSize);

                ViewData["totalpages"] = totalPages;
                ViewData["currentpage"] = page;

            }
            //找到商品主檔
            if (_context.Product != null)
            {
                var Products = await _context.Product.ToListAsync();
                IList<Product> listProducts = new List<Product>();
                foreach(var o in Products)
                {
                    if (listBagsId.Contains(o.strBagsId))
                    {
                        listProducts.Add(o);
                    }
                }
                vm.Products = listProducts;               
            }
            //找到商品的顏色
            if (_context.ProductColor != null)
            {
                var ProductColors = await _context.ProductColor.ToListAsync();
                IList<ProductColor> listProductColors = new List<ProductColor>();
                foreach (var o in ProductColors)
                {
                    if (listBagsId.Contains(o.strBagsId))
                    {
                        listProductColors.Add(o);
                    }
                }
                vm.ProductColors = listProductColors;
                liststrID = vm.ProductColors.Select(row => row.strID).Distinct().ToList();
            }
            //找到商品顏色數量明細
            if (_context.ProductsColorDetail != null)
            {
                var ProductColorDetails = await _context.ProductsColorDetail.ToListAsync();
                IList<ProductsColorDetail> listProductColorDetails = new List<ProductsColorDetail>();
                foreach (var o in ProductColorDetails)
                {
                    if (liststrID.Contains(o.strID))
                    {
                        listProductColorDetails.Add(o);
                    }
                }
                vm.ProductsColorDetails = listProductColorDetails;
            }
            //找輪播設定
            if (_context.CarouselSetting != null)
            {
                vm.CarouselSettings = await _context.CarouselSetting.ToListAsync();
            }


            return View(vm);
        }
        #endregion

        #region 自訂分類 新增
        [HttpGet]
        [Route("Classification/Create/{page}")]
        public async Task<IActionResult> Create(int page)
        {
            ViewData["currentpage"] = page;
            ViewData["classifications"] = await _context.Classification.ToListAsync();
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Route("Classification/Create/{page}")]
        public async Task<IActionResult> Create(Classification model, int page)
        {

            model.Id = GetCFId();
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Main", "Classification", new { page});
            }
            else
            {
                ViewData["currentpage"] = page;
                return View(model);
            }
        }
        #endregion

        #region 自訂分類 編輯
        [HttpGet]
        [Route("Classification/Edit/{page}/{id}")]
        public async Task<IActionResult> Edit(string id, int page)
        {
            if (id == null || _context.Classification == null)
            {
                return NotFound();
            }

            ViewData["classifications"] = await _context.Classification.ToListAsync();

            var model = await _context.Classification.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewData["currentpage"] = page;
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Route("Classification/Edit/{page}/{id}")]
        public async Task<IActionResult> Edit(Classification model, int page)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    if (!ClassificationExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Content(ex.Message);
                    }
                }
                return RedirectToAction("Main", "Classification", new { page});
            }
            else
            {
                ViewData["classifications"] = await _context.Classification.ToListAsync();
                ViewData["currentpage"] = page;
                return View(model);
            }
        }
        #endregion

        #region 自訂分類 刪除
        [HttpGet]
        [Route("Classification/Delete/{page}/{id}")]
        public async Task<IActionResult> Delete(string id, int page)
        {
            if (id == null || _context.Classification == null)
            {
                return NotFound();
            }

            var model = await _context.Classification.FirstOrDefaultAsync(m => m.Id == id);
            ViewData["currentpage"] = page;
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Classification/Delete/{page}/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id, int page)
        {

            if (_context.Classification == null)
            {
                return Problem("Entity set 'YYBagProgramContext.Classification'  is null.");
            }
            //刪除自訂分類
            var classfication = await _context.Classification.FindAsync(id);
            if (classfication != null)
            {
                _context.Classification.Remove(classfication);
            }
            //刪除自訂分類明細
            var isExistClassificationDetail = await _context.ClassificationDetail.AnyAsync(row => row.Id.Equals(id));
            if (isExistClassificationDetail)
            {

                SqlParameter p1 = new SqlParameter("@Id", id);
                string sql = "DELETE ClassificationDetail WHERE Id = @Id";
                await _context.Database.ExecuteSqlRawAsync(sql, p1);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Main", "Classification", new { page });

        }
        #endregion

        #region 自訂分類明細 主頁
        [HttpGet]
        [Route("Classification/DetailMain/{page}/{id}")]
        public async Task<IActionResult> DetailMain(string id, int page)
        {
            if (_context.Product != null)
            {
                ViewData["products"] = await _context.Product.ToListAsync();
            }

            ViewData["id"] = id;
            ViewData["currentpage"] = page;
            if (_context.ClassificationDetail != null)
            {
                if (await _context.Classification.FindAsync(id) != null)
                {
                    ViewData["classificationName"] = await _context.Classification.Where(row => row.Id.Equals(id)).Select(row => row.Name).FirstOrDefaultAsync();
                    return View(await _context.ClassificationDetail.Where(row => row.Id.Equals(id)).ToListAsync());
                }
                return NotFound();
            }
            else
            {
                return Problem("Entity set 'YYBagProgramContext.ClassificationDetail'  is null.");
            }
        }
        #endregion

        #region 自訂分類明細 新增
        [HttpGet]
        [Route("Classification/DetailCreate/{page}/{id}")]
        public async Task<IActionResult> DetailCreate(string id, int page)
        {
            if (_context.Product != null)
            {
                ViewData["products"] = await _context.Product.ToListAsync();
            }

            if(_context.ClassificationDetail != null)
            {
                ViewData["classificationdetail"] = await _context.ClassificationDetail.ToListAsync();
            }

            ViewData["currentpage"] = page;
            
            ClassificationDetail model = new ClassificationDetail();
            model.Id = id;
            
            return View(model);
        }

        [HttpPost, ActionName("DetailCreate")]
        [ValidateAntiForgeryToken]
        [Route("Classification/DetailCreate/{page}/{id}")]
        public async Task<IActionResult> DetailCreate(ClassificationDetail model, int page)
        {
            if (ModelState.IsValid)
            {           
                _context.ClassificationDetail.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("DetailMain", "Classification", new { id = model.Id, page = page });
            }
            else
            {
                ViewData["products"] = await _context.Product.ToListAsync();
                ViewData["classificationdetail"] = await _context.ClassificationDetail.ToListAsync();
                ViewData["currentpage"] = page;
                return View(model);
            }
        }
        #endregion

        #region 自訂分類明細 刪除
        [HttpGet]
        [Route("Classification/DetailDelete/{page}/{id}/{strBagsId}")]
        public async Task<IActionResult> DetailDelete(string id, string strBagsId, int page)
        {
            if(id == null || strBagsId == null)
            {
                return NotFound(); 
            }

            ViewData["currentpage"] = page;

            var model = await _context.ClassificationDetail.FindAsync(id, strBagsId);

            return View(model);
        }

        [HttpPost, ActionName("DetailDelete")]
        [ValidateAntiForgeryToken]
        [Route("Classification/DetailDelete/{page}/{id}/{strBagsId}")]
        public async Task<IActionResult> DetailDeleteConfirmed(string id, string strBagsId, int page)
        {
            if (_context.ClassificationDetail == null)
            {
                return Problem("Entity set 'YYBagProgramContext.ClassificationDetail'  is null.");
            }

            var model = await _context.ClassificationDetail.FindAsync(id, strBagsId);

            if (model !=  null)
            {
                _context.ClassificationDetail.Remove(model);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("DetailMain", "Classification", new { id, page});

        }
        #endregion

        private string GetCFId()
        {
            string result = string.Empty;

            //年月
            string strYearMonth = DateTime.Now.ToString("yyyyMM");

            SqlParameter p1 = new SqlParameter("@YearMonth", strYearMonth);

            var sql = @"SELECT * FROM [Classification] WHERE SUBSTRING(Id, 3, 6) = @YearMonth";

            var data = _context.Classification.FromSqlRaw(sql, p1).ToList();

            if (data.Count == 0)
            {
                result = "CF" + strYearMonth + "01";
            }
            else
            {

                string temp = (Int16.Parse(data.Max(row => row.Id.Substring(row.Id.Length - 2, 2))) + 1).ToString("D2");
                result = "CF" + strYearMonth + temp;
            }

            return result;
        }

        private bool ClassificationExists(string id)
        {
            return (_context.Classification?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        //[Route("Classification/GetImgUrl/{strBagsId}")]
        public async Task<JsonResult> GetImgUrl(string strbagsid)
        {
            string result = string.Empty;

            if (_context.Product != null)
            {
                result = await _context.Product.Where(row => row.strBagsId.Equals(strbagsid)).Select(row => row.strImageUrl).FirstOrDefaultAsync() ?? string.Empty;
            }

            return Json(new { strImageUrl = result });
        }

    }
}
