//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SBlog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Post
    {
        public Post()
        {
            this.PostPraises = new HashSet<PostPrais>();
            this.PostTags = new HashSet<PostTag>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public bool IsPublished { get; set; }
        public string TagsStr { get; set; }
        public string SEOTitle { get; set; }
        public string SEOKeyword { get; set; }
        public string SEODescription { get; set; }
        public int ViewCount { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public int PraiseCount { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual ICollection<PostPrais> PostPraises { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<PostTag> PostTags { get; set; }
    }
}
