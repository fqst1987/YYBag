using Newtonsoft.Json;
using System;
using System.Data;
using YYBagProgram.Enums;

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

        public static string UploadImg(IWebHostEnvironment enviroment, IFormFileCollection target)
        {
            string targetpath = string.Empty;
            if (target != null && target.Count() > 0)
            {
                var uploadFolder = Path.Combine(enviroment.WebRootPath, "upload", "monthlyhot", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("D2"));
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
                }                   
            }
            return targetpath;
        }

        public static string UploadImg(IWebHostEnvironment enviroment, IFormFileCollection target, string oriImageUrl)
        {
            string targetpath = string.Empty;
            if (target != null && target.Count() > 0)
            {
                var uploadFolder = Path.Combine(enviroment.WebRootPath, "upload", "monthlyhot", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("D2"));
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                //刪除舊的
                File.Delete(oriImageUrl);

                foreach (var file in target)
                {
                    string strFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    targetpath = Path.Combine(uploadFolder, strFileName);
                    using (var fileStream = new FileStream(targetpath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                targetpath = oriImageUrl;
            }
            return targetpath;
        }
    }
}
