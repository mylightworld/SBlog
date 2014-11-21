using SBlog.Helpers;
using SBlog.Models;
using SBlog.XmlRpc.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SBlog.Business
{
    public class AccountBll
    {
        //注册成功时，发送激活邮件
        public static bool SentRegisterEmail(string userName,string email,string code)
        {
            try
            {
                var templatePath = "/content/templates/registeremailActivation.html";
                var emailContent = FileHelper.GetFile(templatePath);
                if (string.IsNullOrEmpty(emailContent))
                {
                    return false;
                }

                var activateUrl = CommonHelper.GetDomain();
                activateUrl += string.Format("/account/activate?code={0}", EncrypHelper.Encode(code));
                activateUrl = activateUrl.Replace("+", "%2B");//防止在加密解密里因为+转化为空格产生的错误

                var siteName = GlobalHelper.SiteName;
                emailContent = emailContent.Replace("[[0]]", userName).Replace("[[1]]", activateUrl).Replace("[[2]]", siteName);
                var result = EmailHelper.Send(email, "请激活完成注册！-"+GlobalHelper.SiteName, emailContent);
                return result;
            }
            catch
            {
                return false;
            }
        }

        //重新发送激活邮件或绑定新邮箱
        public static bool SentActivateEmail(string userName, string email, string code)
        {
            try
            {
                var templatePath = "/content/templates/registeremailActivation.html";
                var emailContent = FileHelper.GetFile(templatePath);
                if (string.IsNullOrEmpty(emailContent))
                {
                    return false;
                }

                var activateUrl = CommonHelper.GetDomain();
                activateUrl += string.Format("/account/activate?code={0}", EncrypHelper.Encode(code));
                activateUrl = activateUrl.Replace("+", "%2B");//防止在加密解密里因为+转化为空格产生的错误

                var siteName = GlobalHelper.SiteName;
                emailContent = emailContent.Replace("[[0]]", userName).Replace("[[1]]", activateUrl).Replace("[[2]]",siteName);
                var result = EmailHelper.Send(email, "请点击链接完成激活！-" +siteName, emailContent);
                return result;
            }
            catch
            {
                return false;
            }
        }

        //重新发送激活邮件或绑定新邮箱
        public static bool FogetPassword(string userName, string email, string code)
        {
            try
            {
                var templatePath = "/content/templates/ForgotPassword.html";
                var emailContent = FileHelper.GetFile(templatePath);
                if (string.IsNullOrEmpty(emailContent))
                {
                    return false;
                }

                var activateUrl = CommonHelper.GetDomain();
                activateUrl += string.Format("/account/resetpassword?code={0}", EncrypHelper.Encode(code));
                activateUrl = activateUrl.Replace("+", "%2B");//防止在加密解密里因为+转化为空格产生的错误

                var siteName = GlobalHelper.SiteName;
                emailContent = emailContent.Replace("[[0]]", userName).Replace("[[1]]", activateUrl).Replace("[[2]]", siteName);
                var result = EmailHelper.Send(email, "请点击链接重置密码！-" + siteName, emailContent);
                return result;
            }
            catch
            {
                return false;
            }
        }

        #region MetaWeblogApi

        public static UserInfo GetUserInfo(string key, string username, string password)
        {
            UserInfo item = null;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp =  EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    item = new UserInfo();
                    item.UserId = user.Id.ToString();
                    item.NickName = user.UserName;
                    item.Email = user.Email;
                }
            }

            return item;
        }

        #endregion

    }
}