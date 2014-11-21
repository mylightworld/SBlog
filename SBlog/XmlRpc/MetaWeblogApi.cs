using System;
using XmlRpcMvc;
using XmlRpcMvc.Common;
using SBlog.XmlRpc.Models;
using SBlog.Business;

namespace SBlog.XmlRpc
{
    public class MetaWeblogApi : IXmlRpcService
    {
        [XmlRpcMethod("metaWeblog.newPost")]
        public string NewPost(string blogid, string username, string password, PostInfo post, bool publish)
        {
            var item = PostBll.NewPost(blogid, username, password, post, publish);

            return item;
        }

        [XmlRpcMethod("metaWeblog.editPost")]
        public bool EditPost(string postid, string username, string password, PostInfo post, bool publish)
        {
            var item = PostBll.EditPost(postid, username, password, post, publish);

            return item;
        }

        [XmlRpcMethod("metaWeblog.getPost")]
        public PostInfo GetPost(string postid, string username, string password)
        {
            var item = PostBll.GetPost(postid, username, password);

            return item;
        }

        [XmlRpcMethod("metaWeblog.getCategories")]
        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            var items = PostBll.GetCategories(blogid, username, password);

            return items;
        }

        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public PostInfo[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            var items = PostBll.GetRecentPosts(blogid, username, password, numberOfPosts);

            return items;
        }

        [XmlRpcMethod("metaWeblog.newMediaObject")]
        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            var item = PostBll.NewMediaObject(blogid, username, password, mediaObject);

            return item;
        }

        [XmlRpcMethod("blogger.deletePost")]
        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            var item = PostBll.DeletePost(key, postid, username, password, publish);

            return item;
        }

        [XmlRpcMethod("blogger.getUsersBlogs")]
        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            var item = PostBll.GetUsersBlogs(key, username, password);

            return item;
        }

        [XmlRpcMethod("blogger.getUserInfo")]
        public UserInfo GetUserInfo(string key, string username, string password)
        {
            var item = AccountBll.GetUserInfo(key, username, password);

            return item;
        }
    }
}