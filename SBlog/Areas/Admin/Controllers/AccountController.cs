using SBlog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBlog.Areas.Admin.Controllers
{
    [AdminLoginFilter(Roles="admin")]
    public class AccountController : BaseController
    {
        // GET: Admin/Account
        public ActionResult Index()
        {
            var items = db.Users.ToList();

            return View(items);
        }
    }
}