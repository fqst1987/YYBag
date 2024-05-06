using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using YYBagProgram.Models;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;
using YYBagProgram.Models.ViewModel;
using YYBagProgram.Service.Interface;
using System.Globalization;
using YYBagProgram.Comm;
using System.Web;
using System.Security.Claims;
using MailKit;
using YYBagProgram.Enums;
using YYBagProgram.Service;

namespace YYBagProgram.Controllers
{

    //Member
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ICryptographyService _cryptoService;
        private readonly ISendEmailService _sendEmailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokenService _tokenService;

        public MemberController
            (
            IMemberService memberservice, 
            ICryptographyService cryptographyservice, 
            ISendEmailService sendemailservice, 
            IHttpContextAccessor contextAccessor,
            TokenService tokenService
            )
        {
            _memberService = memberservice;
            _cryptoService = cryptographyservice;
            _sendEmailService = sendemailservice;
            _contextAccessor = contextAccessor;
            _tokenService = tokenService;
        }

        #region 會員中心
        //client
        [HttpGet]
        //[Authorize]
        public IActionResult MemberCenterAdminister()
        {
            return View();
        }

        public IActionResult MemberCenterClient()
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
            string id = _memberService.GoogleApiClientId;
            ViewData["google_client_id"] = id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginPostViewModel vm)
        {
            string strEmail = vm.strMemberGuid;
            string strPassword = vm.strMemberPassWord;

            bool isMemberExist = await _memberService.CheckMemberInfo(strEmail, strPassword, false);
            /*
              確認身分是否正確
              錯誤時顯示查無此註冊手機或密碼錯誤
              正確的時候進入上一個畫面
            */

            if (!string.IsNullOrEmpty(strEmail))
            {
                if (isMemberExist)
                {                
                    var member = _memberService.GetMmebers(strEmail).Result.FirstOrDefault();

                    var claims = new List<Claim>()
                    {
                        new Claim (ClaimTypes.Name, member.strMemberName),
                        new Claim (ClaimTypes.Email, member.strMemberEmail),
                        new Claim ("MemberId", member.strMemberId)                    
                    };

                    var memberrole = _memberService.GetMmeberRoles(member.strMemberId).Result.FirstOrDefault();                 

                    if (memberrole != null)
                    {
                        AllEnums.MemberRole MemberRole = memberrole.Role;
                        claims.Add(new Claim(ClaimTypes.Role, MemberRole.ToString()));
                    }


                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    //成功後轉跳至首頁
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    ViewBag.ErrorMessage = "查無此註冊Email或密碼錯誤";
                    return View();
                }

            }

            return View();
        }

        //google登入
        public async Task<IActionResult> ValidGoogleLogin()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
            if (payload == null)
            {
                // 驗證失敗
                ViewBag.ErrorMessage = "驗證 Google 授權失敗";
                return RedirectToAction("Login", "Member");
            }
            else
            {
                //1.找看看是否有這個email的帳戶 沒有就寫入
                bool isMemberExist = await _memberService.CheckMemberInfo(payload.Email, string.Empty, true);
                if (!isMemberExist)
                {
                    Members newmember = new Members()
                    {
                        strMemberEmail = payload.Email,
                        strMemberName = payload.Name,
                        strMemberPassWord = null,
                        strMemberPhone = null,
                        dateBirthday = null,
                        isGoogleAccount = true,
                        isReview = true,
                    };

                    await _memberService.AddMember(newmember);
                }

                //驗證成功
                var member = (await _memberService.GetMmebers(payload.Email)).FirstOrDefault();
                var memberrole = (await _memberService.GetMmeberRoles(member.strMemberId)).FirstOrDefault();

                var claims = new List<Claim>()
                {
                    new Claim (ClaimTypes.Name, payload.Name),
                    new Claim (ClaimTypes.Email, payload.Email),
                    new Claim ("MemberId", member.strMemberId),
                    new Claim (ClaimTypes.Role, memberrole.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            }

            return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Index", "Home");
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
        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Members member)
        {

            if (await _memberService.isMemberEmailExist(member.strMemberEmail))
            {
                ModelState.AddModelError("strMemberEmail", "此信箱已被註冊");
                return View(member);
            }

            if (ModelState.IsValid)
            {
                //取得密碼的hash值
                var password = member.strMemberPassWord ?? string.Empty;
                var hashpassword = _cryptoService.HashPassword(password);
                member.strMemberPassWord = hashpassword;

                await _memberService.AddMember(member);
                return RedirectToAction("Login", "Member");
            }
            else
            {
                return View(member);
            }
        }
        #endregion

        #region 會員角色 主頁
        [HttpGet]
        [Route("Member/ManageMemberShipMain/{page}")]
        public async Task<IActionResult> ManageMemberShipMain(int page = 1)
        {
            int pagesize = 20;
            MemberRoleViewModel vm = new MemberRoleViewModel();


            var memberroles = await _memberService.GetMmeberRoles();
            var totalpages = (int)Math.Ceiling(memberroles.Count / (double)pagesize);
            var paginatedProducts = memberroles.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            var members = await _memberService.GetMmebers();
            vm.MemberRoles = paginatedProducts;
            vm.Members = members;
            vm.CurremtPage = page;
            vm.TotalPage = totalpages;

            return View(vm);

        }
        #endregion

        #region 會員角色 編輯
        [HttpGet]
        [Route("Member/ManageMemberShipEdit/{page}/{MemberId}")]
        public async Task<IActionResult> ManageMemberShipEdit(string MemberId, int page)
        {
            MemberRoleViewModel vm = new MemberRoleViewModel();

            if (MemberId != null && MemberId.Length > 0)
            {
                vm.Members = await _memberService.GetMmebers(MemberId);
                vm.MemberRoles = await _memberService.GetMmeberRoles(MemberId);

                vm.CurremtPage = page;
                vm.TotalPage = 0;

            }
            return View(vm);
        }

        [HttpPost, ActionName("ManageMemberShipEdit")]
        [Route("Member/ManageMemberShipEdit/{page}/{MemberId}")]
        public async Task<IActionResult> ManageMemberShipEdit(MemberRoleViewModel vm, int page)
        {          
            if (ModelState.IsValid)
            {
                MemberRole model = vm.MemberRoles?.FirstOrDefault() ?? new MemberRole();
                await _memberService.UpdateMember(model);

                return RedirectToAction("ManageMemberShipMain", "Member", new { page });
            }
            else
            {
                return View(vm);
            }           
        }
        #endregion

        #region 忘記密碼
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        /// <summary>
        /// 寄送重設密碼信件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Member/sendMailTokenForReset")]
        public async Task<IActionResult> sendMailTokenForReset(string email)
        {
            DateTime expirationTime;
            string result = await _sendEmailService.SendResetEmail(email);
            string token = result.Split("|", StringSplitOptions.RemoveEmptyEntries).First();
            DateTime.TryParseExact(result.Split("|", StringSplitOptions.RemoveEmptyEntries).Last(), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out expirationTime);
            expirationTime = expirationTime.AddMinutes(30);

            if (!string.IsNullOrEmpty(token))
            {
                //紀錄token
                await _tokenService.AddToken(token, expirationTime);

                return Json(new { token = token, expirationTime = expirationTime ,msg = "請於30分鐘內重設密碼，逾期失效" });
            }
            else
            {
                return Json(new { msg = "發生了問題，請稍後再試" });
            }
        }
        #endregion

        #region 重設密碼
        [HttpGet]
        public async Task<IActionResult> ResetPwd(string token)
        {
            ResetPasswordViewModel vm = new ResetPasswordViewModel();
            
            string verify = string.Empty;

            //檢查是否有驗證碼
            if (string.IsNullOrEmpty(token))
            {
                ViewData["ErrorMessage"] = HttpUtility.JavaScriptStringEncode("缺少驗證碼");
                return View();
            }

            string SecretKey = _memberService.SecretKey;
            string decodeToken = HttpUtility.UrlEncode(token);

            try
            {
                verify = Methods.DecryptBase64String(decodeToken, SecretKey);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(vm);
            }

            string[] verifyParts = verify.Split("|", StringSplitOptions.RemoveEmptyEntries);

            //取出email
            string email = verifyParts[0];

            vm.Email = email;

            //檢查是否超時
            if (!await _tokenService.isTokenExpire(token))
            {
                ViewData["ErrorMsg"] = "超過驗證碼有效時間，請重寄驗證碼";
                return View(vm);
            }

            //檢查token是否已使用過
            if(!await _tokenService.isTokenTaken(token))
            {
                ViewData["ErrorMsg"] = "驗證碼已使用過，請重寄驗證碼";
                return View(vm);
            }

            // 驗證碼檢查成功，加入 Session
            _contextAccessor.HttpContext.Session.SetString("token", decodeToken);

            vm.Email = email;
            return View(vm);

        }

        [HttpPost, ActionName("ResetPwd")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPwd(ResetPasswordViewModel vm)
        {
            if(ModelState.IsValid)
            {
                // 檢查帳號 Session 是否存在
                if (HttpContext.Session.GetString("userEmail") == null || HttpContext.Session.GetString("userEmail")?.Length == 0)
                {
                    return NotFound();
                }

                Members member = (await _memberService.GetMmebers(vm.Email)).FirstOrDefault() ?? new Members();
                try
                {
                    await _memberService.UpdateMember(member);
                    string token = HttpContext.Session.GetString("token") ?? string.Empty;

                    //移除已完成的
                    await _tokenService.RemoveUsedTokens(token);
                    HttpContext.Session.Remove("token");
                    return RedirectToAction("Login", "Member");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View(vm);
                }                    
            }
            else
            {
                ViewBag.ErrorMessage = "發生錯誤";
                return View(vm);
            }
        }
        #endregion

        #region 驗證帳號
        [HttpGet]
        public IActionResult EnableMember()
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }
            return View();
        }

        /// <summary>
        /// 寄送信箱認證信件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Member/sendMailTokenForReview")]
        public async Task<IActionResult> sendMailTokenForReview(string email)
        {
            DateTime expirationTime;
            string result = await _sendEmailService.SendReviewEmail(email);
            string token = result.Split("|", StringSplitOptions.RemoveEmptyEntries).Last();
            DateTime.TryParseExact(result.Split("|", StringSplitOptions.RemoveEmptyEntries).First(), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out expirationTime);
            expirationTime = expirationTime.AddMinutes(30);

            if (!string.IsNullOrEmpty(token))
            {
                //紀錄token
                await _tokenService.AddToken(token, expirationTime);

                return Json(new { token = token, expirationTime = expirationTime, msg = "請於30分鐘內重設密碼，逾期失效" });
            }
            else
            {
                return Json(new { msg = "發生了問題，請稍後再試" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReviewEmail(string token)
        {
            string verify = string.Empty;

            //檢查是否有驗證碼
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = HttpUtility.JavaScriptStringEncode("缺少驗證碼");
                return RedirectToAction("EnableMember", "Member");
            }

            string SecretKey = _memberService.SecretKey;
            string decodeToken = HttpUtility.UrlEncode(token);

            try
            {
                verify = Methods.DecryptBase64String(decodeToken, SecretKey);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("EnableMember", "Member");
            }

            //檢查是否超時
            if (!await _tokenService.isTokenExpire(token))
            {
                TempData["ErrorMsg"] = "超過驗證碼有效時間，請重寄驗證碼";
                await _tokenService.RemoveExpiredTokens();
                return RedirectToAction("EnableMember", "Member");
            }

            //檢查token是否已使用過
            if (!await _tokenService.isTokenTaken(token))
            {
                TempData["ErrorMsg"] = "驗證碼已使用過，請重寄驗證碼";
                return RedirectToAction("EnableMember", "Member");
            }

            //刪除token
            await _tokenService.RemoveUsedTokens(token);

            return View();
        }
        #endregion

        /// <summary>
        /// 確認email是否註冊
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Member/CheckMemberEmail")]
        public async Task<IActionResult> CheckMemberEmail(string email)
        {
            bool isexist = await _memberService.isMemberEmailExist(email);

            return Json(isexist);
        }

        /// <summary>
        /// 檢查是否有這個會員
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Member/CheckMember")]
        public async Task<IActionResult> CheckMember(string email, string password)
        {
            string hashpassword = string.Empty;
            if (!string.IsNullOrEmpty(password))
            {
                hashpassword = _cryptoService.HashPassword(password);
            }
            return Json(await _memberService.CheckMemberInfo(email, hashpassword, false));
        }

        /// <summary>
        /// 檢查會員帳號是否被啟用
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Member/CheckMemberEnable")]
        public async Task<IActionResult> CheckMemberEnable(string account)
        {
            return Json (await _memberService.isMemberEnable(account));

        }

    }
}
