using System.Security.Cryptography;
using System.Text;

namespace YYBagProgram.Models
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
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

        public static bool VerifyHashedPassword(string password, string hashedpassword)
        {
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
