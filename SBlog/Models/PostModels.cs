using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBlog.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage="请填写标题！")]
        [StringLength(200,ErrorMessage="标题不允许超过200个字符！")]
        [Display(Name="标题")]
        public string Title { get; set; }

        [StringLength(500,ErrorMessage="简介不允许超过500个字符！")]
        [Display(Name = "简介")]
        public string ShortDesc { get; set; }

        [Display(Name = "内容")]
        public string Description { get; set; }

        [Display(Name = "是否发布")]
        public bool IsPublished { get; set; }

        [Display(Name = "SEO标题")]
        public string SEOTitle { get; set; }
        [Display(Name = "SEO关键词")]
        public string SEOKeyword { get; set; }
        [Display(Name = "SEO描述")]
        public string SEODescription { get; set; }

        [Required(ErrorMessage="请选择一个分类！")]
        [Range(1, int.MaxValue, ErrorMessage = "请选择一个分类！")]
        [Display(Name = "所属分类")]
        public int CategoryId { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "修改时间")]
        public DateTime UpdateTime { get; set; }
    }
}