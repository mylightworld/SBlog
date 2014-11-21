using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string GetDomain(HttpContext current = null)
        {
            try
            {
                current = current ?? System.Web.HttpContext.Current;

                var activateUrl = current.Request.Url.AbsoluteUri;
                activateUrl = activateUrl.Substring(0, activateUrl.IndexOf("/", activateUrl.IndexOf("//") + 2));

                return activateUrl;
            }
            catch
            {
                return string.Empty;
            }
        }

        //根据邮箱类型，返回邮箱登录网址
        public static string GetEMailWebSite(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }

            email = email.Trim();

            var match = Regex.Match(email, @"@(\w+([-.]\w+)*\.\w+([-.]\w+)*)");
            if (match.Success)
            {
                var eMailWebSites = System.Configuration.ConfigurationManager.AppSettings["EMailWebSites"];
                if (string.IsNullOrEmpty(eMailWebSites))//如abc@qq.com->http://mail.qq.com;abc@163.com->http://mail.163.com
                {
                    return "http://mail." + match.Groups[1].Value;
                }
                else
                {
                    var configs = eMailWebSites.Split(',');
                    foreach (var config in configs)
                    {
                        if (config.Split('|')[0] == (match.Groups[0].Value))
                        {
                            return config.Split('|')[1];
                        }
                    }

                    return "http://mail." + match.Groups[1].Value;
                }
            }

            return string.Empty;
        }

        public static int GetRandom(int min=100000, int max=999999)
        {
            var item = new System.Random(System.DateTime.Now.Millisecond * 1000).Next(min, max);

            return item;
        }

        //根据多媒体的类型，生成一个Url
        public static string GetMediaUrl(string type)
        {
            var result = "";

            if (type.StartsWith("image"))
            {
                var extName = type.Split('/').Length > 1 ? type.Split('/')[1] : "jpg";
                result = string.Format("/up/images/{0}/{1}/{2}/{3}.{4}", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, CommonHelper.GetRandom(), extName);
            }

            return result;
        }

        public static string GenerateImage(string type, byte[] bits)
        {
            var url = CommonHelper.GetMediaUrl(type);

            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            //保存图片前，确保相应文件夹已经创建
            var tmpUrl = "";
            var dirs = url.Substring(0, url.LastIndexOf('/'));
            foreach (var part in dirs.Split('/'))
            {
                if (!string.IsNullOrEmpty(part))
                {
                    tmpUrl += "/" + part;
                    if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(tmpUrl)))
                    {
                        System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(tmpUrl));
                    }
                }
            }
            
            //将byte数组保存成图片

            MemoryStream ms = new MemoryStream(bits); // MemoryStream创建其支持存储区为内存的流；MemoryStream属于System.IO类           
            ms.Position = 0;
            Image img = Image.FromStream(ms);
            img.Save(System.Web.HttpContext.Current.Server.MapPath(url));
            ms.Close();

            return url;
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