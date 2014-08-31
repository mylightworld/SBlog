using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SBlog.Helpers
{
    public class EmailHelper
    {
        public static bool Send(string to,string subject,string body)
        {
            MailAddress from = new MailAddress("sblog@7z8she.com", "sblog"); //邮件的发件人

            MailMessage mail = new MailMessage();

            //设置邮件的标题
            mail.Subject = subject;

            //设置邮件的发件人
            mail.From = from;

            //设置邮件的收件人
            mail.To.Add(to);

            //设置邮件的内容
            mail.Body = body;
            //设置邮件的格式
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            //设置邮件的发送级别
            mail.Priority = MailPriority.Normal;

            SmtpClient client = new SmtpClient();
            //设置用于 SMTP 事务的主机的名称，填IP地址也可以了
            client.Host = "smtp.exmail.qq.com";
            //client.Port = 465;
            //client.EnableSsl = true;
            //设置用于 SMTP 事务的端口，默认的是 25
            //client.Port = 25;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("sblog@7z8she.com", "Bluesky1234");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}