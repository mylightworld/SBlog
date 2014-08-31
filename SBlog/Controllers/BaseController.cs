using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBlog.Controllers
{
    public class BaseController : Controller
    {
        protected SBlog.Models.SBlogEntities db = new Models.SBlogEntities();
        protected SBlog.Helpers.CookieHelper cookie = new Helpers.CookieHelper();
    }
}