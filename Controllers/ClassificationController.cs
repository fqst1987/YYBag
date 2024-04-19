using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using YYBagProgram.Data;
using YYBagProgram.Models;

namespace YYBagProgram.Controllers
{
    //[Authorize(Roles = "Administer")]
    //Classification
    public class ClassificationController : Controller
    {
        private readonly YYBagProgramContext _context;
        private readonly IWebHostEnvironment _environment;

        public ClassificationController(YYBagProgramContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        #region 自訂分類 主頁
        [HttpGet]
        public async Task<IActionResult> Main()
        {
            return _context.Classification != null ?
                        View(await _context.Classification.ToListAsync()) :
                        Problem("Entity set 'YYBagProgramContext.Classification'  is null.");
        }

        [HttpGet]
        public async Task<IActionResult> ClassificationMain(string strClassificationId)
        {
            CFViewModel vm = new CFViewModel();
            List<string> listBagsId = new List<string>();
            List<string> liststrID = new List<string>();

            //找到分類
            if (_context.Classification != null && strClassificationId != null)
            {
                vm.Classfications = await _context.Classification.ToListAsync();
                vm.Classification = await _context.Classification.Where(row => row.Id.Equals(strClassificationId)).FirstOrDefaultAsync();
                ViewData["titlemsg"] = vm.Classification?.Name;
            }
            //找到分類明細
            if (_context.ClassificationDetail != null)
            {
                vm.ClassificationDetails = await _context.ClassificationDetail.Where(row => row.Id.Equals(strClassificationId)).ToListAsync();
                //找到分類明細中的商品項目
                listBagsId = vm.ClassificationDetails.Select(row => row.strBagsId).Distinct().ToList();             
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
            //找當月主打
            if (_context.MonthlyHots != null)
            {
                int iyear = DateTime.Now.Year;
                int imonth = DateTime.Now.Month;
                vm.MonthlyHots = await _context.MonthlyHots.Where(row => row.Year == iyear && row.Month == imonth).ToListAsync();
            }


            return View(vm);
        }
        #endregion

        #region 自訂分類 新增
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Classification model)
        {

            model.Id = GetCFId();
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Main));
            }

            return View(model);
        }
        #endregion

        #region 自訂分類 編輯
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Classification == null)
            {
                return NotFound();
            }

            var model = await _context.Classification.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Classification model)
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
                        throw;
                    }
                }
                return RedirectToAction("Main", "Products");
            }
            return View(model);
        }
        #endregion

        #region 自訂分類 刪除
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Classification == null)
            {
                return NotFound();
            }

            var model = await _context.Classification.FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
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
            return RedirectToAction("Main", "Classification");

        }
        #endregion

        #region 自訂分類明細 主頁
        [HttpGet]
        [Route("Classification/DetailMain/{id}")]
        public async Task<IActionResult> DetailMain(string id)
        {
            ViewData["products"] = await _context.Product.ToListAsync();
            ViewData["id"] = id;
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
        //[[Route("Classification/DetailCreate/{id}")]
        public async Task<IActionResult> DetailCreate(string id)
        {
            ViewData["products"] = await _context.Product.ToListAsync();
            ViewData["classificationdetail"] = await _context.ClassificationDetail.ToListAsync();
            ClassificationDetail model = new ClassificationDetail();
            model.Id = id;
            
            return View(model);
        }

        [HttpPost, ActionName("DetailCreate")]
        [ValidateAntiForgeryToken]
        //[[Route("Classification/DetailCreate/{id}")]
        public async Task<IActionResult> DetailCreate(ClassificationDetail model)
        {
            if (ModelState.IsValid)
            {           
                _context.ClassificationDetail.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("DetailMain", "Classification", new { id = model.Id });
            }
            ViewData["products"] = await _context.Product.ToListAsync();
            ViewData["classificationdetail"] = await _context.ClassificationDetail.ToListAsync();
            return View(model);
        }
        #endregion

        #region 自訂分類明細 刪除
        [HttpGet]
        [Route("Classification/DetailDelete/{id}/{strBagsId}")]
        public async Task<IActionResult> DetailDelete(string id, string strBagsId)
        {
            if(id == null || strBagsId == null)
            {
                return NotFound(); 
            }
            var model = await _context.ClassificationDetail.FindAsync(id, strBagsId);

            return View(model);
        }

        [HttpPost, ActionName("DetailDelete")]
        [ValidateAntiForgeryToken]
        [Route("Classification/DetailDelete/{id}/{strBagsId}")]
        public async Task<IActionResult> DetailDeleteConfirmed(string id, string strBagsId)
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
            return RedirectToAction("DetailMain", "Classification", new { id});

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

    }
}
