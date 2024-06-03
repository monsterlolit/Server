using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{
    public class shoes
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Color { get; set; } = "";
        public string Brand { get; set; } = "";
        public decimal Cost { get; set; }
        public double Size { get; set; }
        public byte[] Image { get; set; }
       
  
    }
    

   
}
