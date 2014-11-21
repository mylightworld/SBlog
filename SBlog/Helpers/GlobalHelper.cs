using SBlog.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBlog.Helpers
{
    public class GlobalHelper
    {
        public static string SiteName 
        {
            get
            {
                var siteName = "SBlog";

                //using (var db = new Models.SBlogEntities())
                //{
                //    var item = db.Configs.FirstOrDefault(x => x.Name == "SiteName");
                //    if (item != null)
                //    {
                //        siteName = item.Val;
                //    }
                //}

                return siteName;
            }

            set
            {
                using (var db = new Models.SBlogEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == CookieHelper.UserId);
                    if (user != null)
                    {
                        //管理员才可以修改
                        if (user.UserRoles.Count(x => x.Role.Code == "admin")>0)
                        {
                            var item = db.Configs.FirstOrDefault(x => x.Name == "SiteName");
                            if (item != null)
                            {
                                item.Val = value.ToTrim();
                            }
                            else
                            {
                                item = new Models.Config();
                                item.Name = "SiteName";
                                item.Val = value.ToTrim();
                            }

                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public static string Url
        {
            get
            {
                var url = "";

                using (var db = new Models.SBlogEntities())
                {
                    var item = db.Configs.FirstOrDefault(x => x.Name == "Url");
                    if (item != null)
                    {
                        url = item.Val;
                    }
                }

                return url;
            }

            set
            {
                using (var db = new Models.SBlogEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == CookieHelper.UserId);
                    if (user != null)
                    {
                        //管理员才可以修改
                        if (user.UserRoles.Count(x => x.Role.Code == "admin") > 0)
                        {
                            var item = db.Configs.FirstOrDefault(x => x.Name == "Url");
                            if (item != null)
                            {
                                item.Val = value.ToTrim();
                            }
                            else
                            {
                                item = new Models.Config();
                                item.Name = "Url";
                                item.Val = value.ToTrim();
                            }

                            db.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}