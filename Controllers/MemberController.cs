using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YYBagProgram.Data;
using YYBagProgram.Models;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;

namespace YYBagProgram.Controllers
{

    public class MemberController : Controller
    {
        private readonly YYBagProgramContext _context;
        private readonly IWebHostEnvironment _enviroment;
        private readonly IConfiguration _configuration;
        private readonly MemberSerivce _memberserivce;

        public MemberController(YYBagProgramContext context, IWebHostEnvironment enviroment, IConfiguration configuration, MemberSerivce memberserivce)
        {
            _context = context;
            _enviroment = enviroment;
            _configuration = configuration;
            _memberserivce = memberserivce;
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
            string id = _configuration["GoogleApiClientId"];
            ViewData["google_client_id"] = id;
            return View();
        }

        public bool CheckMember(string account, string password)
        {
            bool result = false;

            if (_context.Member != null && _context.Member.Any())
            {
                var member = _context.Member.FirstOrDefault(m => m.strMemberEmail.Equals(account) || m.strMemberPhone.Equals(account));

                if (member != null)
                {
                    string strHashPassword = MemberSerivce.HashPassword(password);
                    bool isPasswordCorrect = MemberSerivce.VerifyHashedPassword(password, strHashPassword);

                    if (isPasswordCorrect)
                    {
                        result = true;
                    }
                }                   
            }
            return result;
        }

        //google登入
        public IActionResult ValidGoogleLogin()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
            if (payload == null)
            {
                // 驗證失敗
                ViewData["Msg"] = "驗證 Google 授權失敗";
            }
            else
            {
                //驗證成功，取使用者資訊內容
                ViewData["Msg"] = "驗證 Google 授權成功" + "<br>";
                ViewData["Msg"] += "Email:" + payload.Email + "<br>";
                ViewData["Msg"] += "Name:" + payload.Name + "<br>";
                ViewData["Msg"] += "Picture:" + payload.Picture;
            }

            return RedirectToAction("HomePage", "Home");
        }
        private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? formCredential, string? formToken, string? cookiesToken)
        {
            // 檢查空值
            if (formCredential == null || formToken == null && cookiesToken == null)
            {
                return null;
            }

            GoogleJsonWebSignature.Payload? payload;
            try
            {
                // 驗證 token
                if (formToken != cookiesToken)
                {
                    return null;
                }

                // 驗證憑證
                IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                string GoogleApiClientId = Config.GetSection("GoogleApiClientId").Value;
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { GoogleApiClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(formCredential, settings);
                if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
                {
                    return null;
                }
                if (payload.ExpirationTimeSeconds == null)
                {
                    return null;
                }
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
                    if (now > expiration)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return payload;
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

        #region 未登入
        public IActionResult NoLogin()
        {
            return RedirectToAction("Login", "Member");
        }
        #endregion

        #region 註冊
        [HttpGet]
        public async Task<IActionResult> Register() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Members member)
        {
            if (_memberserivce.isAccountRegister(member.strMemberPhone ?? string.Empty, member.strMemberEmail))
            {
                ModelState.AddModelError("strMemberEmail", "此信箱/手機已被註冊");
                return View(member);
            }

            if (ModelState.IsValid)
            {
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

                //取得密碼的hash值
                var password = member.strMemberPassWord ?? string.Empty;
                var hashpassword = MemberSerivce.HashPassword(password);
                member.strMemberPassWord = hashpassword;

                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction("Home", "Hompage");
            }
            else
            {
                return View(member);
            }
        }
        #endregion

    }
}
