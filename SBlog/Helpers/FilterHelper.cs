using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBlog.Helpers
{
    public class LoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CookieHelper.IsLogin && !filterContext.RequestContext.HttpContext.Request.RawUrl.ToLower().StartsWith("/account/login"))
            {
                filterContext.Result = new RedirectResult("/account/login");
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class AdminLoginFilter : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CookieHelper cookie = new CookieHelper();
            var validate = CookieHelper.IsLogin;
            HttpResponseBase response = filterContext.HttpContext.Response;

            //检查角色
            if (validate)
            {
                var roles = cookie["Roles"];
                foreach (var R in Roles.Split(','))
                {
                    foreach (var r in roles.Split(','))
                    {
                        if (!string.IsNullOrEmpty(R + r) && R.Equals(r))
                        {
                            validate = false;
                            break;
                        }
                    }

                    if (!validate)
                    {
                        break;
                    }
                }
            }

            if (!CookieHelper.IsLogin && !validate && !filterContext.RequestContext.HttpContext.Request.RawUrl.ToLower().StartsWith("/admin/login"))
            {
                response.Redirect("/admin/login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}