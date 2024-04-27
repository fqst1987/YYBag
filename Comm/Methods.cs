using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using System.Web;


namespace YYBagProgram.Comm
{
    public static class Methods
    {
       
        public static string DataTable2Json(DataTable dt)
        {
            if (dt == null)
            {
                return string.Empty;
            }
            
            string strJson = JsonConvert.SerializeObject(dt, Formatting.Indented);

            return strJson;
        }

        public static string GetImagesURL(IFormFileCollection imagefiles)
        {
            List<string> listImgFileName = new List<string>();
            string strImageURLs = string.Empty;
            if (imagefiles != null && imagefiles.Count > 0)
            {
                if (ValidateFile(imagefiles))
                {
                    foreach (IFormFile file in imagefiles)
                    {
                        listImgFileName.Add(file.FileName);

                    }
                    strImageURLs = String.Join(";", listImgFileName);
                }
            }
            return strImageURLs;
        }

        private static bool ValidateFile(IFormFileCollection files)
        {
            foreach (IFormFile file in files)
            {
                if (file.Length < 1024 * 1024 && file.ContentType.StartsWith("image/"))
                    continue;
                else
                    return false;
            }
            return true;
        }

        public static string UploadImg(IWebHostEnvironment enviroment, IFormFileCollection target, string folderName)
        {
            string targetpath = string.Empty;
            string strImageURLs = string.Empty;
            List<string> listImgFileName = new List<string>();
            if (target != null && target.Count() > 0)
            {
                var uploadFolder = Path.Combine(enviroment.WebRootPath, "upload", folderName, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("D2"));
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach(var file in target)
                {
                    string strFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    targetpath = Path.Combine(uploadFolder, strFileName);
                    using (var fileStream = new FileStream(targetpath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    listImgFileName.Add(targetpath);
                    
                }
                strImageURLs = String.Join(";", listImgFileName);
            }
            return strImageURLs;
        }

        public static string UploadImg(IWebHostEnvironment enviroment, IFormFileCollection target, string folderName, string oriImageUrl)
        {
            string targetpath = string.Empty;
            string strImageURLs = string.Empty;
            List<string> listImgFileName = new List<string>();
            if (target != null && target.Count() > 0)
            {
                var uploadFolder = Path.Combine(enviroment.WebRootPath, "upload", folderName, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("D2"));
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                //刪除舊的
                string[] strOriimgurls = oriImageUrl.Split(";", StringSplitOptions.RemoveEmptyEntries);
                foreach( var stroriimgurl in strOriimgurls)
                {
                    File.Delete(stroriimgurl);
                }

                foreach (var file in target)
                {
                    string strFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    targetpath = Path.Combine(uploadFolder, strFileName);
                    using (var fileStream = new FileStream(targetpath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    listImgFileName.Add(targetpath);
                }
                strImageURLs = String.Join(";", listImgFileName);
                targetpath = strImageURLs.Trim();
            }
            else
            {
                targetpath = oriImageUrl;
            }
            return targetpath;
        }

        public static string EncryptStringToBase64(string plainText, string SecretKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);
                Array.Resize(ref keyBytes, 32);

                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes;
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(plainText);
                        csEncrypt.Write(buffer, 0, buffer.Length);
                        csEncrypt.FlushFinalBlock();
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }

                return HttpUtility.UrlEncode(Convert.ToBase64String(encryptedBytes));
            }
        }

        public static string DecryptBase64String(string base64String, string SecretKey)
        {
            byte[] encryptedBytes = Convert.FromBase64String(HttpUtility.UrlDecode(base64String));
            using (Aes aesAlg = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);
                Array.Resize(ref keyBytes, 32);

                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
