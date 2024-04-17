using Newtonsoft.Json;
using System.Data;
using YYBagProgram.Enums;

namespace YYBagProgram.Comm
{
    public class Uitility
    {

        //DataTable轉成Json格式
        public static string DataTable2Json(DataTable dt)
        {
            if (dt == null)
            {
                return string.Empty;
            }
            
            string strJson = JsonConvert.SerializeObject(dt, Formatting.Indented);

            return strJson;
        }
    }
}
