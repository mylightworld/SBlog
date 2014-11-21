using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using SBlog.Models;
using SBlog.Helpers;

namespace SBlog.Areas.Admin.Controllers
{
    [AdminLoginFilter(Roles = "admin,editor,author")]
    public class PostController : BaseController
    {
        protected static readonly ILog myLog4Net = LogManager.GetLogger(typeof(PostController));

        // GET: Admin/Post
        public ActionResult Index(int page=1)
        {
            var pageSize = 10;
            var items = db.Posts.OrderByDescending(x => x.UpdateTime).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(items);
        }

        public ActionResult Create()
        {
            var model = new PostModel();

            //没有分类，则创建一个默认分类
            if (db.Categories.Count() <= 0)
            {
                var defaultCat = new Category();
                defaultCat.Name = "默认分类";
                defaultCat.UpdateTime = defaultCat.CreateTime = DateTime.Now;
                defaultCat.UserId = CookieHelper.UserId;

                db.Categories.Add(defaultCat);
                db.SaveChanges();

                defaultCat.CategoriesStr = "," + defaultCat.Id + ",";
                db.SaveChanges();
            }

            return View("CreateOrEdit",model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PostModel model,FormCollection collection)
        {
            int? cid = null;
            if (!string.IsNullOrEmpty(collection["category"]))
            {
                //一般会按name将元素的值用“，”连接起来，因此，需要取该参数的最后一个合法值
                var cidsStr = Request.Params["category"].Trim();
                foreach (var cidStr in cidsStr.Split(','))
                {
                    var tmpCid = 0;
                    if (int.TryParse(cidStr, out tmpCid))
                    {
                        cid = tmpCid;
                    }
                }
            }
            else
            {
                ModelState.AddModelError("CategoryId","请选择一个分类！");
            }

            if (ModelState.IsValid)
            {
                var item = new Post();
                item.Title = model.Title.Trim();
                item.ShortDesc = model.ShortDesc.ToTrim();
                item.Description = model.Description.ToTrim();
                if (item.ShortDesc == null)
                {
                    item.ShortDesc = item.Description.ToChText(50);
                }
                item.UpdateTime = item.CreateTime = DateTime.Now;
                item.CategoryId = model.CategoryId;
                item.SEOTitle = model.SEOTitle.ToTrim();
                item.SEOKeyword = model.SEOKeyword.ToTrim();
                item.SEODescription = model.SEODescription.ToTrim();

                db.Posts.Add(item);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.cid = cid;

            return View("CreateOrEdit",model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.Posts.Find(id);
            if (item == null)
            {
                return View("NoResource");
            }

            var model = new PostModel();
            model.Id = item.Id;
            model.Title = item.Title;
            model.ShortDesc = item.ShortDesc;
            model.Description = item.Description;
            model.SEOTitle = item.SEOTitle;
            model.SEOKeyword = item.SEOKeyword;
            model.SEODescription = item.SEODescription;
            if (item.CategoryId != null)
            {
                model.CategoryId = item.CategoryId.Value;
            }

            ViewBag.cid = model.CategoryId;

            //没有分类，则创建一个默认分类
            if (db.Categories.Count() <= 0)
            {
                var defaultCat = new Category();
                defaultCat.Name = "默认分类";
                defaultCat.UpdateTime = defaultCat.CreateTime = DateTime.Now;
                defaultCat.UserId = CookieHelper.UserId;

                db.Categories.Add(defaultCat);
                db.SaveChanges();
                defaultCat.CategoriesStr = "," + defaultCat.Id + ",";
                db.SaveChanges();
            }

            return View("CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PostModel model, FormCollection collection)
        {
            var item = db.Posts.Find(model.Id);
            if (item == null)
            {
                return View("NoResource");
            }

            int? cid = null;
            if (!string.IsNullOrEmpty(collection["category"]))
            {
                //一般会按name将元素的值用“，”连接起来，因此，需要取该参数的最后一个合法值
                var cidsStr = Request.Params["category"].Trim();
                foreach (var cidStr in cidsStr.Split(','))
                {
                    var tmpCid = 0;
                    if (int.TryParse(cidStr, out tmpCid))
                    {
                        cid = tmpCid;
                    }
                }
            }
            else
            {
                ModelState.AddModelError("CategoryId", "请选择一个分类！");
            }

            if (ModelState.IsValid)
            {
                item.Title = model.Title.Trim();
                item.ShortDesc = model.ShortDesc.ToTrim();
                item.Description = model.Description.ToTrim();
                if (item.ShortDesc == null)
                {
                    item.ShortDesc = item.Description.ToChText(50);
                }
                item.UpdateTime  = DateTime.Now;
                item.CategoryId = model.CategoryId;
                item.SEOTitle = model.SEOTitle.ToTrim();
                item.SEOKeyword = model.SEOKeyword.ToTrim();
                item.SEODescription = model.SEODescription.ToTrim();

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.cid = cid;

            return View("CreateOrEdit", model);
        }

        public ActionResult Delete(int id,int?page)
        {
            var item = db.Posts.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return View("NoResource");
            }

            var title = item.Title;

            try
            {
                db.Posts.Remove(item);

                db.SaveChanges();

                TempData["message"] = "删除文章：【" + title + "】成功！";
            }
            catch(Exception e)
            {
                TempData["message"] = "系统异常，删除文章失败！";
                myLog4Net.Debug(string.Format("删除{0}时系统异常，编号为：{1}，异常信息为：{2}！",item.Title,id,e.ToString()));
            }

            return RedirectToAction("Index", new { page=page });
        }
    }
}