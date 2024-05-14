using System.Text;
using YYBagProgram.Service.Interface;
using System.Security.Cryptography;

namespace YYBagProgram.Service
{
    public class CryptographyService : ICryptographyService
    {
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
    }
}
