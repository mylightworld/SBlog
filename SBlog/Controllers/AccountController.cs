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
using Newtonsoft.Json;

namespace SBlog.Controllers
{
    public class AccountController : BaseController
    {

        //
        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
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
                    cookie["UserName", 1] = item.UserName;
                    cookie["Roles", 1] = string.Join(",", item.UserRoles.Select(x => x.Role.Code));

                    return Redirect("/");
                }
            }

            return View(model);
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
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
                var activateModel = new ActivateModel { Id = item.Id, Email = item.Email, CreateTime = DateTime.Now.ToUniversalTime() };
                var code = JsonConvert.SerializeObject(activateModel);
                AccountBll.SentRegisterEmail(item.UserName,item.Email,code);

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

            if (string.IsNullOrEmpty(deCode))
            {
                TempData["message"] = "链接参数不正确！请重新发送激活邮件！";
                return RedirectToAction("ActivateFailed");
            }

            var js = new JavaScriptSerializer();
            var model = JsonConvert.DeserializeObject<ActivateModel>(deCode);

            if (model.CreateTime <= DateTime.Now.AddHours(-1) && false)//时间转换问题，暂时不检验
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

        [LoginFilter]
        public ActionResult SentActivateEmail()
        {
            var item = db.Users.FirstOrDefault(x => x.Id == CookieHelper.UserId);
            if (item == null)
            {
                return View("NoResource");
            }

            if (item.EmailActivated)
            {
                return View();
            }

            var activateModel = new ActivateModel { Id = item.Id, Email = item.Email, CreateTime = DateTime.Now.ToUniversalTime() };
            var code = JsonConvert.SerializeObject(activateModel);
            var result = AccountBll.SentActivateEmail(item.UserName,item.Email,code);
            if (result)
            {
                return RedirectToAction("SentActivateEmailSuccess", new { email=item.Email});
            }
            else
            {
                return RedirectToAction("SentActivateEmailFailed", new { email=item.Email});
            }
        }

        public ActionResult SentActivateEmailSuccess(string email)
        {
            ViewBag.email = email;
            ViewBag.emailWebSite = CommonHelper.GetEMailWebSite(email);

            return View();
        }

        public ActionResult SentActivateEmailFailed(string email)
        {
            ViewBag.email = email;

            return View();
        }

        [LoginFilter]
        public ActionResult Settings()
        {
            var item = db.Users.FirstOrDefault(x => x.Id == CookieHelper.UserId);

            if (item == null)
            {
                return View("NoResource");
            }

            ViewBag.message = TempData["message"];

            return View(item);
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ChangePassword()
        {
            var item = db.Users.FirstOrDefault(x => x.Id == CookieHelper.UserId);
            if (item == null)
            {
                return View("NoResource");
            }

            var model = new ChangePasswordModel();
            model.UserName = item.UserName;

            return View(model);
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var item = db.Users.FirstOrDefault(x => x.Id == CookieHelper.UserId);
                if (item == null)
                {
                    return View("NoResource");
                }

                var oldPwdEncryp = EncrypHelper.MD5(model.Password.Trim());
                if (oldPwdEncryp != item.Password)
                {
                    ModelState.AddModelError("Password", "输入的旧密码不正确！");
                    return View(model);
                }

                item.Password = EncrypHelper.MD5(model.NewPassword.Trim());
                item.UpdateTime = DateTime.Now;
                db.SaveChanges();

                TempData["message"] = "修改密码成功！";
                return RedirectToAction("Settings");
            }
            
            return View(model);
        }

        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordModel();

            return View(model);
        }

        [HttpPost]
        //
        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //发送一封邮件到邮箱
                var item = db.Users.FirstOrDefault(x => x.Email == model.Email);
                if (item == null)
                {
                    return View("NoResource");
                }

                var verifyModel = new ActivateModel { CreateTime = DateTime.Now, Email = model.Email, Id = item.Id };
                var code = JsonConvert.SerializeObject(verifyModel);
                var result = AccountBll.FogetPassword(item.UserName, item.Email, code);
                if (result)
                {
                    return RedirectToAction("SentForgotPasswordEmailSuccess");
                }
                else
                {
                    return RedirectToAction("SentForgotPasswordEmailFailed");
                }
            }

            return View(model);
        }

        public ActionResult SentForgotPasswordEmailSuccess(string email)
        {
            ViewBag.email = email;

            return View();
        }

        public ActionResult SentForgotPasswordEmailFailed(string email)
        {
            ViewBag.email = email;
            ViewBag.emailWebSite = CommonHelper.GetEMailWebSite(email);

            ViewBag.message = TempData["message"];
            return View();
        }

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword()
        {
            var code = Request.Params["code"];
            var deCode = EncrypHelper.Decode(code);

            if (string.IsNullOrEmpty(deCode))
            {
                TempData["message"] = "链接参数不正确！请重新尝试！";
                return RedirectToAction("ResetPasswordFailed");
            }

            var js = new JavaScriptSerializer();
            var verifyModel = JsonConvert.DeserializeObject<ForgotPasswordVerifyModel>(deCode);

            if (verifyModel.CreateTime <= DateTime.Now.AddHours(-1) && false)//时间转换问题，暂时不检验
            {
                TempData["message"] = "链接已经过期！请重新发送忘记密码邮件！";
                return RedirectToAction("ResetPasswordFailed");
            }

            var item = db.Users.FirstOrDefault(x => x.Id == verifyModel.Id);
            if (item == null)
            {
                TempData["message"] = "链接参数不正确！请重新尝试！";
                return RedirectToAction("ResetPasswordFailed");
            }

            ViewBag.code = code;

            var model = new ResetPasswordModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var code = Request.Params["code"];
            var deCode = EncrypHelper.Decode(code);

            if (string.IsNullOrEmpty(deCode))
            {
                TempData["message"] = "链接参数不正确！请重新尝试！";
                return RedirectToAction("ResetPasswordFailed");
            }

            var js = new JavaScriptSerializer();
            var verifyModel = JsonConvert.DeserializeObject<ForgotPasswordVerifyModel>(deCode);

            if (verifyModel.CreateTime <= DateTime.Now.AddHours(-1) && false)//时间转换问题，暂时不检验
            {
                TempData["message"] = "链接已经过期！请重新发送忘记密码邮件！";
                return RedirectToAction("ResetPasswordFailed");
            }

            var item = db.Users.FirstOrDefault(x => x.Id == verifyModel.Id);
            if (item == null)
            {
                TempData["message"] = "链接参数不正确！请重新尝试！";
                return RedirectToAction("ResetPasswordFailed");
            }

            if (ModelState.IsValid)
            {
                item.Password = EncrypHelper.MD5(model.NewPassword.Trim());
                item.UpdateTime = DateTime.Now;

                db.SaveChanges();

                return RedirectToAction("ResetPasswordSuccess");
            }

            return View(model);
        }

        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        public ActionResult ResetPasswordFailed()
        {
            ViewBag.message = TempData["message"];
            return View();
        }

        [LoginFilter]
        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            cookie["UserId", -1] = null;
            cookie["UserName", -1] = null;

            return RedirectToAction("Index", "Home");
        }

    }
}