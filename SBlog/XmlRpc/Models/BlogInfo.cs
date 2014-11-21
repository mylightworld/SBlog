using System.Runtime.Serialization;

namespace SBlog.XmlRpc.Models
{
    public class BlogInfo
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
        
		[DataMember(Name = "blogid")]
		public string Blogid { get; set; }
		
		[DataMember(Name = "blogName")]
        public string BlogName { get; set; }
    }
}
