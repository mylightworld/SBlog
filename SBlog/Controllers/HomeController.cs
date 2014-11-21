using SBlog.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBlog.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var page = 3;

            var first = db.Posts.OrderByDescending(x => x.UpdateTime).FirstOrDefault();
            ViewBag.first = first;

            var items = db.Posts.OrderByDescending(x => x.UpdateTime).Skip(1).Take(page).ToList();

            return View(items);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}