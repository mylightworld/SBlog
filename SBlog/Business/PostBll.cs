using SBlog.Models;
using SBlog.XmlRpc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SBlog.Helpers;
using System.Data.Common;
using System.Data;

namespace SBlog.Business
{
    public class PostBll
    {

        #region MetaWeblogApi

        public static BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            BlogInfo[] items = null;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    items = new BlogInfo[1] { new BlogInfo{ Blogid = "1", BlogName = GlobalHelper.SiteName,Url=GlobalHelper.Url} };
                }
            }

            return items;
        }

        public static bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            var result = false;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if(user != null)
                {
                    var postId = Convert.ToInt32(postid);
                    var item = db.Posts.FirstOrDefault(x => x.Id == postId && x.UserId == user.Id);
                    if (item != null)
                    {
                        if (item.Category != null)
                        {
                            item.Category.PostCount = (item.Category.PostCount > 2 ? item.Category.PostCount - 1 : 0);
                        }

                        db.Posts.Remove(item);

                        try
                        {
                            db.SaveChanges();

                            result = true;
                        }
                        catch
                        {
                            result = false;
                        }
                    }
                }
            }

            return result;
        }

        public static PostInfo[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts=30)
        {
            PostInfo[] items = null;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    items = db.Posts.Where(x => x.UserId == user.Id).OrderByDescending(x => x.CreateTime)
                        .Select(x => new PostInfo { Title = x.Title, Description = x.Description, DateCreated = x.CreateTime.Value }).Take(numberOfPosts).ToArray();
                }
            }

            return items;
        }

        public static string NewPost(string blogid, string username, string password, PostInfo post, bool publish)
        {
            string result = null;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    //没有分类，则创建一个默认分类
                    int? defaultCatId = null;
                    var defaultCat = db.Categories.FirstOrDefault(x => x.Name == "默认分类");
                    if (defaultCat == null)
                    {
                        defaultCat = new Category();
                        defaultCat.Name = "默认分类";
                        defaultCat.UpdateTime = defaultCat.CreateTime = DateTime.Now;
                        defaultCat.UserId = user.Id;

                        db.Categories.Add(defaultCat);
                        db.SaveChanges();

                        defaultCat.CategoriesStr = "," + defaultCat.Id + ",";
                        db.SaveChanges();

                        defaultCatId = defaultCat.Id;
                    }
                    else
                    {
                        defaultCatId = defaultCat.Id;
                    }

                    var item = new Post();
                    item.Title = post.Title.ToTrim();
                    item.Description = post.Description.ToTrim();
                    item.ShortDesc = item.Description.ToChText(50);
                    item.CreateTime = item.UpdateTime = post.DateCreated ?? DateTime.Now;
                    item.UserId = user.Id;
                    item.IsPublished = true;
                    if (post.Categories.Length > 0)
                    {
                        var catName = post.Categories.Last();
                        var category = db.Categories.FirstOrDefault(x => x.Name == catName);
                        if (category != null)
                        {
                            item.CategoryId = category.Id;
                            category.PostCount++;
                        }
                        else
                        {
                            item.CategoryId = defaultCatId;
                            defaultCat.PostCount++;
                        }
                    }
                    else
                    {
                        item.CategoryId = defaultCatId;
                        defaultCat.PostCount++;
                    }

                    db.Posts.Add(item);

                    try
                    {
                        db.SaveChanges();

                        result = item.Id.ToString();
                    }
                    catch
                    {
                        result = null;
                    }
                }
            }

            return result;
        }

        public static bool EditPost(string postid, string username, string password, PostInfo post, bool publish)
        {
            var result = false;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    //没有分类，则创建一个默认分类
                    int? defaultCatId = null;
                    var defaultCat = db.Categories.FirstOrDefault(x => x.Name == "默认分类");
                    if (defaultCat == null)
                    {
                        defaultCat = new Category();
                        defaultCat.Name = "默认分类";
                        defaultCat.UpdateTime = defaultCat.CreateTime = DateTime.Now;
                        defaultCat.UserId = user.Id;

                        db.Categories.Add(defaultCat);
                        db.SaveChanges();

                        defaultCat.CategoriesStr = "," + defaultCat.Id + ",";
                        db.SaveChanges();

                        defaultCatId = defaultCat.Id;
                    }
                    else
                    {
                        defaultCatId = defaultCat.Id;
                    }

                    var postId = Convert.ToInt32(postid);
                    var item = db.Posts.FirstOrDefault(x => x.Id == postId && x.UserId == user.Id);
                    if (item != null)
                    { 
                        item.Title = post.Title.ToTrim();
                        item.Description = post.Description.ToTrim();
                        item.ShortDesc = item.Description.ToChText(50);
                        item.UpdateTime = DateTime.Now;
                        item.IsPublished = publish;
                        if (post.Categories.Length > 0)
                        {
                            var catName = post.Categories.Last();
                            var category = db.Categories.FirstOrDefault(x => x.Name == catName);
                            if (category != null)
                            {
                                item.CategoryId = category.Id;
                                category.PostCount++;
                            }
                            else
                            {
                                item.CategoryId = defaultCatId;
                                defaultCat.PostCount++;
                            }
                        }
                        else
                        {
                            item.CategoryId = defaultCatId;
                            defaultCat.PostCount++;
                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            result = false;
                        }
                    }
                    
                }
            }

            return result;
        }

        public static PostInfo GetPost(string postid, string username, string password)
        {
            PostInfo item = null;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    var postId = Convert.ToInt32(postid);
                    var post = db.Posts.FirstOrDefault(x => x.Id == postId && x.UserId == user.Id);
                    if (post != null)
                    {
                        item = new PostInfo();
                        item.Title = post.Title;
                        item.Description = post.Description;
                        item.DateCreated = post.CreateTime.Value;
                        if (post.Category != null && !string.IsNullOrEmpty(post.Category.CategoriesStr))
                        {
                            var categories = new List<string>();
                            foreach (var catIdStr in post.Category.CategoriesStr.Split(','))
                            {
                                if (!string.IsNullOrEmpty(catIdStr))
                                {
                                    var catId = Convert.ToInt32(catIdStr);
                                    var category = db.Categories.FirstOrDefault(x => x.Id == catId);
                                    if (category != null)
                                    {
                                        categories.Add(category.Name);
                                    }
                                }
                            }

                            if (categories.Count > 0)
                            {
                                item.Categories = categories.ToArray();    
                            }
                        }
                    }
                }
            }

            return item;
        }

        public static CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            CategoryInfo[] items = null;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    items = db.Categories.Select(x => new CategoryInfo { Title = x.Name }).ToArray();
                }
            }

            return items;
        }

        public static MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            MediaObjectInfo item = null;

            using (var db = new SBlogEntities())
            {
                var passwordEncryp = EncrypHelper.MD5(password.ToTrim());
                var userName = username.ToTrim();
                var user = db.Users.FirstOrDefault(x => x.UserName == userName && x.Password == passwordEncryp);
                if (user != null)
                {
                    var medium = new Medium();
                    medium.Name = mediaObject.Name;
                    medium.Type = mediaObject.TypeName;
                    medium.Url = CommonHelper.GenerateImage(medium.Type, mediaObject.Bits);
                    if (!string.IsNullOrEmpty(medium.Url))
                    {
                        item = new MediaObjectInfo { Url = medium.Url };

                        db.Media.Add(medium);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }

            return item;
        }

        #endregion
        
    }
}