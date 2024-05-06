using System.Text;
using System.Security.Cryptography;
using YYBagProgram.Data;
using Microsoft.EntityFrameworkCore;
using YYBagProgram.Models;
using System.Runtime.InteropServices;
using YYBagProgram.Enums;
using YYBagProgram.Service.Interface;
using System.Web;
using MimeKit;
using YYBagProgram.Models.Mail;
using Microsoft.Extensions.Options;
using MailKit.Security;
using MailKit.Net.Smtp;
using YYBagProgram.Comm;
using Microsoft.AspNetCore.Identity;

namespace YYBagProgram.Service
{
    public class MemberService : ISendEmailService, ICryptographyService, IMemberService
    {
        private readonly YYBagProgramContext _context;
        private readonly IConfiguration _configuration;
        private readonly MailSettings _mailSettings;
        private readonly IHttpContextAccessor _contextAccessor;

        public string GoogleApiClientId { get; }
        public string SecretKey { get; }
        public string GmailUserID { get; }
        public string GmailUserPassword { get; }
        public MemberService(YYBagProgramContext context, IConfiguration configuration, IOptions<MailSettings> mailsettings, IHttpContextAccessor httpContext)
        {
            _context = context;
            _configuration = configuration;
            GoogleApiClientId = _configuration["GoogleApiClientId"];
            SecretKey = _configuration["SecretKey"];
            _mailSettings = mailsettings.Value;
            _contextAccessor = httpContext;
        }

        /// <summary>
        /// 將密碼轉為sha256
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            if (password.Length == 0 || password == null)
            {
                return string.Empty;
            }
            //SHA256
            using (var sha256 = SHA256.Create())
            {
                //將密碼轉換為位元組陣列
                var bytes = Encoding.UTF8.GetBytes(password);

                //計算密碼hash
                var hash = sha256.ComputeHash(bytes);

                //將hash轉為16進為字串
                var hexString = BitConverter.ToString(hash, 0, hash.Length).Replace("-", "");

                return hexString;
            }
        }

        /// <summary>
        /// 檢查密碼輸入是否相同
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hashedpassword"></param>
        /// <returns></returns>
        public bool VerifyHashedPassword(string password, string hashedpassword)
        {
            if (password.Length == 0 || password == null)
            {
                return false;
            }

            var bytes = Encoding.UTF8.GetBytes(password);

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bytes);

                var hexString = BitConverter.ToString(hash).Replace("-", "");

                return hexString == hashedpassword;
            }
        }

        /// <summary>
        /// 寄送重製密碼信件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<string> SendResetEmail(string email)
        {
            //取得資料庫內的email
            string EmailTo = _context.Member?.FindAsync(email).Result?.strMemberEmail ?? email;

            // 產生帳號+時間驗證碼
            string verificationData = email + "|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // Encrypt verification data
            string encryptedData = Methods.EncryptStringToBase64(verificationData, SecretKey);

            // Set expiration time to 30 minutes from now
            DateTime expirationTime = DateTime.Now.AddMinutes(30);

            // 網站網址
            var request = _contextAccessor.HttpContext?.Request;
            string webPath = $"{request?.Scheme}://{request?.Host}{request?.PathBase}/";

            // 從信件連結回到重設密碼頁面
            string receivePage = "Member/ResetPwd";

            // 信件內容範本
            string mailContent = "請點擊以下連結，返回網站重新設定密碼，逾期 30 分鐘後，此連結將會失效。<br><br>";
            mailContent = mailContent + "<a href='" + webPath + receivePage + "?token=" + encryptedData + "'  target='_blank'>點此連結</a>";


            //發送信件的請求內容
            MailRequst mailRequest = new MailRequst()
            {
                ToEmail = EmailTo,
                Subject = "[測試] 重設密碼申請信",
                Body = mailContent
            };

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;

            //寄件人資訊
            var mail = new MimeMessage();
            mail.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            mail.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            mail.Subject = mailRequest.Subject;
            mail.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                try
                {
                    await smtp.SendAsync(mail);
                    smtp.Dispose();
                    return expirationTime.ToString("yyyy/MM/dd HH:mm:ss") + "|" + encryptedData;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 寄送信箱認證信件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<string> SendReviewEmail(string email)
        {
            //取得資料庫內的email
            string EmailTo = _context.Member?.FindAsync(email).Result?.strMemberEmail ?? email;

            // 產生帳號+時間驗證碼
            string verificationData = email + "|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // Encrypt verification data
            string encryptedData = Methods.EncryptStringToBase64(verificationData, SecretKey);

            // Set expiration time to 30 minutes from now
            DateTime expirationTime = DateTime.Now.AddMinutes(30);

            // 網站網址
            var request = _contextAccessor.HttpContext?.Request;
            string webPath = $"{request?.Scheme}://{request?.Host}{request?.PathBase}/";

            // 從信件連結回到重設密碼頁面
            string receivePage = "Member/ReviewEmail";

            // 信件內容範本
            string mailContent = "請點擊以下連結來驗證信箱，逾期 30 分鐘後，此連結將會失效。<br><br>";
            mailContent = mailContent + "<a href='" + webPath + receivePage + "?token=" + encryptedData + "'  target='_blank'>點此連結</a>";


            //發送信件的請求內容
            MailRequst mailRequest = new MailRequst()
            {
                ToEmail = EmailTo,
                Subject = "[測試] 信箱認證申請信",
                Body = mailContent
            };

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;

            //寄件人資訊
            var mail = new MimeMessage();
            mail.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            mail.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            mail.Subject = mailRequest.Subject;
            mail.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                try
                {
                    await smtp.SendAsync(mail);
                    smtp.Dispose();
                    return expirationTime.ToString("yyyy/MM/dd HH:mm:ss") + "|" + encryptedData;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 檢查是否有註冊email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> isMemberEmailExist(string email)
        {
            return await _context.Member.AnyAsync(row => row.strMemberEmail.Equals(email.Trim()));
        }

        /// <summary>
        /// 確認登入者是否正確
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberInfo(string account, string password, [Optional] bool isgoogleaccount)
        {
            bool result = false;

            Members member = new Members();

            if (_context.Member != null && _context.Member.Any())
            {
                if (isgoogleaccount || (password.Length == 0 || password == null))
                {
                    member = await _context.Member.FirstOrDefaultAsync(m => m.strMemberEmail.Equals(account));
                    if (member != null)
                    {
                        result = true;
                    }
                }
                else if (!isgoogleaccount && password.Length > 0)
                {
                    string hashPassword = HashPassword(password);
                    member = await _context.Member.FirstOrDefaultAsync(m => m.strMemberEmail.Equals(account));

                    if (member != null)
                    {
                        bool isPasswordCorrect = VerifyHashedPassword(password, hashPassword);

                        if (isPasswordCorrect)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 取得會員信箱是否認證
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<bool> isMemberEnable(string account)
        {
            bool result = false;

            Members member = new Members();

            if (_context.Member != null && _context.Member.Any())
            {
                member = await _context.Member.FirstOrDefaultAsync(m => m.strMemberEmail.Equals(account));
                if (member != null)
                {
                    result = member.isReview;
                }
            }
            return result;
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="model"></param>
        public async Task AddMember<T>(T model)
        {
            var type = typeof(T);
            if (type == typeof(Members))
            {
                Members member = model as Members ?? new Members();
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

                //寫入會員Role表
                MemberRole memberRole = new MemberRole();
                memberRole.MemberId = member.strMemberId;
                memberRole.Role = AllEnums.MemberRole.Client;

                _context.Member.Add(member);
                _context.MemberRole.Add(memberRole);

                await _context.SaveChangesAsync();
            }

        }

        /// <summary>
        /// 刪除會員
        /// </summary>
        /// <param name="model"></param>
        public async Task RemoveMember<T>(T model)
        {
            var type = typeof(T);

            if (type == typeof(MemberRole))
            {
                MemberRole memberrole = model as MemberRole ?? new MemberRole();
                _context.MemberRole.Remove(memberrole);
  
            }
            else if (type == typeof(Members))
            {
                Members member = model as Members ?? new Members();
                MemberRole memberrole = await _context.MemberRole.FindAsync(member.strMemberId) ?? new MemberRole();

                _context.Member.Remove(member);
                _context.MemberRole.Remove(memberrole);
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public async Task UpdateMember<T>(T model)
        {
            var type = typeof(T);

            if (type == typeof(MemberRole))
            {
                MemberRole memberrole = model as MemberRole ?? new MemberRole();
                _context.MemberRole.Update(memberrole);

            }
            else if (type == typeof(Members))
            {
                Members member = model as Members ?? new Members();
                _context.Member.Update(member);

            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 取得會員資料
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<IList<Members>> GetMmebers([Optional] string target)
        {
            IList<Members> model = new List<Members>();

            if (target != null && target.Length > 0)
            {
                var model1 = await _context.Member.Where(row => row.strMemberEmail.Equals(target)).ToListAsync();
                if (model1 != null && model1.Count > 0)
                    model = model1;

                var model2 = await _context.Member.Where(row => row.strMemberId.Equals(target)).ToListAsync();
                if (model2 != null && model2.Count > 0)
                    model = model2;
                return model;
            }
            else
            {
                return await _context.Member.ToListAsync();
            }
        }

        /// <summary>
        /// 取得會員角色資料
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<IList<MemberRole>> GetMmeberRoles([Optional] string target)
        {
            if (target != null && target.Length > 0)
            {
                return await _context.MemberRole.Where(row => row.MemberId.Equals(target)).ToListAsync();
            }
            else
            {
                return await _context.MemberRole.ToListAsync();
            }
        }

    }
}
