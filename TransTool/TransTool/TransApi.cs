using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Tool
{
    public class TransApi
    {
        private static string TRANS_API_HOST { get; } =
            "http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}";

        private static string Url =
            "http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}";

        private string appid ;
        private string salt;
        private string BaiduPublieKey = "12345678";

        public const int MaxTextLength = 400;

        public int WaitiTime { get; set; } = 10000;
        public TransApi(string appid, string salt)
        {
            this.appid = appid;
            this.salt = salt;
        }


        public string GetTransResult(string query, string from, string to)
        {
            string q = System.Web.HttpUtility.UrlEncode(query, System.Text.Encoding.UTF8);

            string sign = GetMD5(appid + query + salt + BaiduPublieKey);

            string address = string.Format(Url, q, from, to, appid, salt, sign);

            HttpWebRequest request;
            request = (HttpWebRequest)HttpWebRequest.Create(address);
            request.Method = "GET";
            request.ProtocolVersion = HttpVersion.Version11;
            //request.Connection = "keep-alive";  
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.861.0 Safari/535.2";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Timeout = WaitiTime;
            //request.Headers.Add("Connection", "keep-alive");   

            request.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.Headers.Add("Accept-Charset", "GBK,utf-8;q=0.7,*;q=0.3");
            request.CookieContainer = new CookieContainer();

            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            Stream st;
            st = response.GetResponseStream();
            GZipStream temp = null;
            StreamReader stReader;
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                temp = new GZipStream(st, CompressionMode.Decompress, true);
                stReader = new StreamReader(temp, Encoding.Default);
            }
            else
            {
                stReader = new StreamReader(st, Encoding.Default);
            }

            string text;
            text = stReader.ReadToEnd();

            stReader.Close();
            if (temp != null)
                temp.Close();
            st.Close();

            BaiduResult r = JsonGet(text);
            if (r != null && r.trans_result != null && r.trans_result.Length > 0)
                return r.trans_result[0].dst;
            return "";
        }



        /// <summary>    
        /// 加密成32位小写的MD5    
        /// </summary>    
        /// <param name="myString">传入需要加密的字符串</param>    
        /// <returns>返回加密后的字符串</returns>    
        public static string GetMD5(string myString)
        {
#pragma warning disable 618
            var hashPasswordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(myString, "MD5");
#pragma warning restore 618
            return hashPasswordForStoringInConfigFile?.ToLower();
        }


        public static BaiduResult JsonGet(string jsonString)
        {
            if (jsonString.Length > 0)
            {
                var ms = new MemoryStream(Encoding.Default.GetBytes(jsonString));
                return (BaiduResult)new DataContractJsonSerializer(typeof(BaiduResult)).ReadObject(ms);
            }
            return null;
        }

    }

    public class BaiduResult
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string from;
        [DataMember(Order = 1)]
        public string to;
        public class Trans_result
        {
            public string src;
            public string dst;
        }
        [DataMember(Order = 2)]
        public Trans_result[] trans_result;
    }

    public enum LangType
    {
        auto=0,
        zh,
        en,
        cht,
    }
}
