using SBlog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBlog.Business
{
    public class AccountBll
    {
        public static bool SentRegisterEmail(string userName,string email,string activateUrl)
        {
            try
            {
                var templatePath = "/content/templates/registeremailActivation.html";
                var emailContent = FileHelper.GetFile(templatePath);
                if (string.IsNullOrEmpty(emailContent))
                {
                    return false;
                }

                emailContent = emailContent.Replace("[[0]]", userName).Replace("[[1]]", activateUrl);
                var result = EmailHelper.Send(email, "请激活完成注册！-SBlog", emailContent);
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}