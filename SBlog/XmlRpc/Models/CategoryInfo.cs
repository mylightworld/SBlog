using System.Runtime.Serialization;

namespace SBlog.XmlRpc.Models
{
    public class CategoryInfo
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }
		
		[DataMember(Name = "title")]
        public string Title { get; set; }
    }
}
