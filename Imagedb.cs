using System.ComponentModel.DataAnnotations;

namespace Server
{
    public class Imagedb
    {
        [Key]
        public int ImageID { get; set; }
        public byte[] Image { get; set; }
        public virtual ICollection<Shoes> Shoes { get; set; }
    }
}
