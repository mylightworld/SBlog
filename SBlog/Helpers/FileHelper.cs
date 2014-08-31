using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SBlog.Helpers
{
    public class FileHelper
    {
        public static string GetFile(string relativePath,string encoding="utf-8")
        {
            var absolutePath = HttpContext.Current.Server.MapPath(relativePath);
            var resutl = string.Empty;

            string line = null;
            StreamReader sr = new StreamReader(absolutePath, Encoding.GetEncoding(encoding));
            while ((line = sr.ReadLine()) != null)
            {
                resutl += line;
            }

            return resutl;
        }
    }
}