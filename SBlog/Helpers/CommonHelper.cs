using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SBlog.Helpers
{
    public class CommonHelper
    {
        public static string GetCity(string ip=null)
        {
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }

            try
            {
                DataSet ds = new DataSet();  //读取XML数据到DataSet  
                ds.ReadXml("http://www.youdao.com/smartresult-xml/search.s?type=ip&q=" + ip);
                //获得location列的数据
                string location = ds.Tables[0].Rows[0]["location"].ToString();
                location = location.Replace(" ", "");

                return location;
            }
            catch
            {
                return string.Empty;
            }
        }

        #region 基本方法

        public static string HttpGet(string Url, string postDataStr, CookieContainer cookie, string code = "gbk")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=" + code;
            request.CookieContainer = cookie;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(code));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public static string HttpPost(string Url, string postDataStr, CookieContainer cookie, string code = "gbk")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            request.CookieContainer = cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding(code));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Cookies = cookie.GetCookies(response.ResponseUri);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(code));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        #endregion
    }
}