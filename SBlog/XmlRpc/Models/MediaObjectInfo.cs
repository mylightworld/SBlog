using System.Runtime.Serialization;

namespace SBlog.XmlRpc.Models
{
    public class MediaObjectInfo
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
