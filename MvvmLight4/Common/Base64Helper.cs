using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace MvvmLight4.Common
{
    public class Base64Helper
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }

        ///解码
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        public static string GetTime()
        {
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.baidu.com");
                var response = myHttpWebRequest.GetResponse();
                string todaysDates = response.Headers["date"];
                return DateTime.ParseExact(todaysDates,
                                           "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                           CultureInfo.InvariantCulture.DateTimeFormat,
                                           DateTimeStyles.AssumeUniversal).ToString("yyyy-MM-dd");
            }
            catch (Exception e)
            {
                Console.WriteLine("发生异常：{0}", e.ToString());
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

        }
    }
}
