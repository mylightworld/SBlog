using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SBlog.Helpers
{
    public class CookieHelper
    {
        private string _cookiePre{get{return ConfigurationManager.AppSettings["CookiePre"].ToString(); }}

        public void SetCookie(string name,string value,int type=1)//-1过期删除，1保存
        {
            var cookieName = _cookiePre + name;
            var encrypValue = EncrypHelper.Encode(value);
            HttpCookie cookie = new HttpCookie(cookieName);
            if (type == 1)
            {
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            else
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
            }
            cookie[cookieName] = encrypValue;
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public string GetCookie(string name)
        {
            var cookieName = _cookiePre + name;
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(cookieName);
            if (cookie == null)
            {
                return string.Empty;
            }

            var value = cookie.Value;
            if (string.IsNullOrEmpty(value) || !value.Contains("="))
            {
                return string.Empty;
            }

            value = value.Substring(value.IndexOf('=')+1);
            var decrypValue = EncrypHelper.Decode(value);

            return decrypValue;
        }

        public string this[string name]
        {
            get
            {
                return this.GetCookie(name);
            }
        }

        public object this[string name, int type = 1]
        {
            set
            {
                this.SetCookie(name, value==null?string.Empty:value.ToString(), type);
            }
        }

        public static bool IsLogin
        {
            get
            {
                var co = new CookieHelper();

                return !string.IsNullOrEmpty(co["UserId"]);
            }
        }

        public static int UserId
        {
            get
            {
                var co = new CookieHelper();

                return Convert.ToInt32(co["UserId"]);
            }
        }

        public static string UserName
        {
            get
            {
                var co = new CookieHelper();

                return co["UserName"];
            }
        }
    }
}