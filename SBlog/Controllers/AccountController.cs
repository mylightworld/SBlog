using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SBlog.Models;
using SBlog.Helpers;
using SBlog.Business;
using System.Web.Script.Serialization;

namespace SBlog.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var passwordEncryp = EncrypHelper.MD5(model.Password);

                var item = db.Users.FirstOrDefault(x => 
                    (x.UserName == model.UserName.Trim() || x.Email == model.UserName.Trim())
                    );
                if (item == null || item.IsDeleted)
                {
                    ModelState.AddModelError("", "没有找到相应账户！");
                }
                else if (item.Password != passwordEncryp)
                {
                    ModelState.AddModelError("", "账号或密码不正确！");
                }
                else
                {
                    //保存登录状态到cookie
                    cookie["UserId", 1] = item.Id;

                    return Redirect("/");
                }
            }

            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var item = db.Users.FirstOrDefault(x => x.Email == model.Email.Trim());
                if (item != null)
                {
                    ModelState.AddModelError("Email", "该邮箱已经有人使用过了！");
                    return View(model);
                }

                item = db.Users.FirstOrDefault(x => x.UserName == model.UserName.Trim());
                if (item != null)
                {
                    ModelState.AddModelError("UserName", "该用户名已经有使用使用过了！");
                    return View(model);
                }

                item = new User();
                item.UserName = model.UserName.Trim();
                item.Password = EncrypHelper.MD5(model.Password.Trim());
                item.CreateTime = DateTime.Now;
                item.Email = model.Email.Trim();
                item.RegisterIp = Request.UserHostAddress;
                item.RegisterCity = CommonHelper.GetCity(item.RegisterIp);
                item.LastLoginIp = item.RegisterIp;
                item.LastLoginCity = item.RegisterCity;
                item.Email = model.Email.Trim();

                db.Users.Add(item);
                db.SaveChanges();

                //登录
                cookie["UserId", 1] = item.Id;

                //发送电子邮件
                var activateUrl = Request.Url.Host;
                var activateModel= new ActivateModel{ Id=item.Id,Email=item.Email,CreateTime=DateTime.Now};
                var js = new JavaScriptSerializer();
                var code = js.Serialize(activateModel);
                activateUrl += string.Format("?code={0}", EncrypHelper.Encode(code));
                AccountBll.SentRegisterEmail(item.UserName,item.Email,activateUrl);

                return Redirect("/");
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public ActionResult RegisterSuccess(string userName,string email)
        {
            return View();
        }

        //邮件激活
        public ActionResult Activate()
        {
            var code = Request.Params["code"];
            var deCode = EncrypHelper.Decode(code);

            var js = new JavaScriptSerializer();
            var model = js.Deserialize<ActivateModel>(deCode);

            if (model.CreateTime <= DateTime.Now.AddHours(-1))
            {
                TempData["message"] = "链接已经过期！请重新发送激活邮件！";
                return RedirectToAction("ActivateFailed");
            }

            var item = db.Users.FirstOrDefault(x => x.Id == model.Id && x.Email == model.Email);
            if (item == null)
            {
                TempData["message"] = "链接参数不正确！请重新发送激活邮件！";
                return RedirectToAction("ActivateFailed");
            }

            item.EmailActivated = true;
            db.SaveChanges();

            return RedirectToAction("ActivateSuccess");
        }

        public ActionResult ActivateSuccess()
        {
            return View();
        }

        public ActionResult ActivateFailed()
        {
            ViewBag.message = TempData["message"];

            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}