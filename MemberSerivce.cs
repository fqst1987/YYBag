using System.Text;
using YYBagProgram.Data;
using System.Security.Cryptography;
using YYBagProgram.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;
using Newtonsoft.Json.Linq;

namespace YYBagProgram
{
    public class MemberSerivce
    {
        private readonly YYBagProgramContext _context;

        public MemberSerivce(YYBagProgramContext context)
        {
            _context = context;
        }

        //確認是否已註冊過
        public bool isAccountRegister(string? phone, string email)
        {
            bool result = false;

            //檢查看看有沒有相同email
            if (email.Length > 0 && email != null)
            {
                if (_context.Member.Any(row => row.strMemberEmail.Equals(email)))
                    result = true;
            }

            //檢查看看有沒有相同phonenumber
            if (phone.Length > 0 && phone != null)
            {
                if (_context.Member.Any(row => row.strMemberPhone.Equals(phone)))
                    result = true;
            }

            return result;
        }

        //取得密碼的hash值
        public static string HashPassword(string password)
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

        //檢查密碼跟hash值是否相同
        public static bool VerifyHashedPassword(string password, string hashedpassword)
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
    }
}
