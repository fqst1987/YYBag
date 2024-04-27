using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Security.Cryptography;


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
