using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Reflection;
using System.Security.Claims;
using YYBagProgram.Data;
using YYBagProgram.Models;
using Microsoft.AspNetCore.Authorization;
using YYBagProgram.Models.ViewModel;

namespace YYBagProgram.Controllers
{

    public class MemberController : Controller
    {
        private readonly YYBagProgramContext _context;
        private readonly IWebHostEnvironment _enviroment;

        public MemberController(YYBagProgramContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _enviroment = enviroment;
        }

        #region 會員中心
        //client
        [HttpGet]
        [Authorize]
        public IActionResult MemberCenter()
        {
            return View();
        }
        #endregion

        #region 購物車
        [HttpGet]
        [Authorize(Roles = "Client")]
        public IActionResult MemberShopChart()
        {
            return View();
        }
        #endregion

        #region 本月主打
        [HttpGet]
        [Authorize(Roles = "Administer")]
        public IActionResult MonthlyHot()
        {
            return RedirectToAction("Main", "MonthlyHot");
        }
        #endregion

        #region 登入
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginPostViewModel loginPost)
        {

            string strPhone = loginPost.strMemberGuid;
            string strPassword = loginPost.strMemberPassWord;
            string strHashPassword = PasswordHasher.HashPassword(strPassword);
            bool isPasswordCorrect = PasswordHasher.VerifyHashedPassword(strPassword, strHashPassword);
            /*
              確認身分是否正確
              錯誤時顯示查無此註冊手機或密碼錯誤
              正確的時候進入上一個畫面
            */

            if (!string.IsNullOrEmpty(strPhone))
            {
                if (_context.Member.Any(row => row.strMemberPhone.Equals(strPhone)) && isPasswordCorrect)
                {
                    var varCollection = _context.Member.Where(row => row.strMemberPhone.Equals(strPhone)).FirstOrDefault();
                    string strMemberID = varCollection.strMemberId;
                    string strMemberName = varCollection.strMemberName;
                    string strMemberEmail = varCollection.strMemberEmail;
                    string strMemberPhone = varCollection.strMemberPhone;

                    var claims = new List<Claim>
                    {
                        new Claim (ClaimTypes.Name, strMemberID),
                        new Claim (ClaimTypes.Email, strMemberEmail),
                        new Claim (ClaimTypes.MobilePhone, strPhone),
                        new Claim ("FullName", strMemberName)
                    };

                    //如果是系統管理者
                    if (strMemberPhone.Equals("0932513822"))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Administer"));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Client"));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    //成功後轉跳至首頁
                    return RedirectToAction("HomePage", "Home");
                }
                else
                {
                    //失敗的話跳出視窗並回原登入頁面
                    return RedirectToAction("ShowPopup", "Errors");
                }
            }
            
            return View();
        }
        #endregion

        #region 登出
        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("HomePage", "Home");
        }
        #endregion

        public IActionResult NoLogin()
        {
            return RedirectToAction("Login", "Member");
        }

        #region 註冊
        [HttpGet]
        public IActionResult Register() 
        {
            //建立一個新的member
            Members member = new Members();
            member.dateBirthday = DateTime.Now;

            //給一個當年度最新的id
            string strNow = DateTime.Now.ToString("yyyyMM");

            bool isExist = _context.Member.Any(row => row.strMemberId.Substring(0, 6) == strNow);
            if (!isExist)
            {
                member.strMemberId = strNow + "0001";
            }
            else
            {
                var aaa = _context.Member.Where(row => row.strMemberId.Substring(0, 6).Equals(strNow)).ToList();
                var bbb = aaa.Max(row => row.strMemberId.Substring(6, 4));
                var ccc = (int.Parse(bbb) + 1).ToString("D4");
                member.strMemberId = strNow + ccc;
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("strMemberId,strMemberPassWord,strMemberName,strMemberPhone,strMemberEmail,dateBirthday")] Members member)
        {
            //檢查此手機號碼是否已經註冊過
            string strPhone = member.strMemberPhone;

            if (CheckPhoneNumberExist(strPhone)) 
            {
                return RedirectToAction("ShowPoopup", "Error");
            }

            var password = member.strMemberPassWord;

            //取的password的hash值
            var hashpassword = PasswordHasher.HashPassword(password);

            //將取得的hash值寫入member中
            member.strMemberPassWord = hashpassword;

            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Member");

            }
            else
            {
                // 有錯誤，跳出警示畫面
                return View("Error");
            }

        }
        #endregion

        private bool CheckPhoneNumberExist(string strPhone)
        {
            return _context.Member.Any(row => row.strMemberPhone == strPhone);
        }

    }
}
