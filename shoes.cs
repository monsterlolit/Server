using System.ComponentModel.DataAnnotations;

namespace Server
{
    public class Shoes
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Color { get; set; } = "";
        public string Brand { get; set; } = "";
        public decimal Cost { get; set; }
        public double Size { get; set; }
        public int ImageID { get; set; }
        public virtual Imagedb Imagedb { get; set; }
    }
}
