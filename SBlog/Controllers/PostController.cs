using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace SBlog.Controllers
{
    public class PostController : BaseController
    {
        //
        // GET: /Post/
        public ActionResult Index(int page=1)
        {
            var pageSize = 10;

            var tmpItems = db.Posts.OrderByDescending(x => x.UpdateTime).ToList();
            var items = tmpItems.ToPagedList(page, pageSize);

            return View(items);
        }
	}
}