using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBlog.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="请填写分类名称！")]
        [StringLength(50,ErrorMessage="分类名称最多50个字符！")]
        [Display(Name="名称")]
        public string Name { get; set; }
        [Display(Name = "父类")]
        public int? ParentId { get; set; }
        public string CategoriesStr { get; set; }
        public int PostCount { get; set; }
    }
}