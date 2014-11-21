using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SBlog.Helpers
{
    public static class ExtentionHelper
    {
        //去除两边的空格
        public static string ToTrim(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            else
            {
                return str.Trim();
            }
        }

        //获取指定个数的汉字或符号
        //bluesky:2014-11-13
        public static string ToChText(this string content, int count = 50, bool needDots = true)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var result = Regex.Replace(content, @"[^\u4e00-\u9fa5、，。……！]", ""); //只留汉字或标点

            if (needDots)
            {
                if (result.Length > count - 3)
                {
                    result = result.Substring(0, count - 3) + "...";
                }
            }
            else
            {
                if (result.Length > count)
                {
                    result = result.Substring(0, 50);
                }
            }

            return result;
        }
    }
}