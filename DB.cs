using Microsoft.EntityFrameworkCore;
using Server;

namespace Server
{
    public class DB : DbContext
    {
        public DbSet<Shoes> Shoes { get; set; } = null!;
        public DbSet<Imagedb> ImageDBTable { get; set; } = null!;
        public DbSet<Users> Users { get; set; } = null!;
        public DB(DbContextOptions<DB> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shoes>().HasData(
                new Shoes { Id = 1, Name = "Adi2000", Color = "Black", Brand = "Adidas", Cost = 25000, Size = 42, ImageID = 1 },
                new Shoes { Id = 2, Name = "Originals Samba", Color = "Green", Brand = "Adidas", Cost = 1200, Size = 42, ImageID = 2 },
                new Shoes { Id = 3, Name = "ADIZERO SL RUNNING", Color = "White", Brand = "Adidas", Cost = 17500, Size = 42, ImageID = 3 },
                new Shoes { Id = 4, Name = "AIR MAX TERRASCAPE PLUS", Color = "Black", Brand = "Nike", Cost = 16900, Size = 42, ImageID = 4 },
                new Shoes { Id = 5, Name = "Air Zoom Superrep 3", Color = "White", Brand = "Nike", Cost = 31999, Size = 42, ImageID = 5 }
     
                );
            
            modelBuilder.Entity<Imagedb>().HasData(
                new Imagedb {ImageID = 1, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo1.jpg")},
                new Imagedb {ImageID = 2, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo2.jpeg")},
                new Imagedb {ImageID = 3, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo3.jpg")},
                new Imagedb {ImageID = 4, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo4.jpg")},
                new Imagedb {ImageID = 5, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo5.jpg")}
            );
            modelBuilder.Entity<Shoes>()
       .HasOne(s => s.Imagedb) 
       .WithMany(i => i.Shoes) 
       .HasForeignKey(s => s.ImageID);
            modelBuilder.Entity<Users>().HasData(
                    new Users { Id = 1, Login = "1", Password = "1" },
                    new Users { Id = 2, Login = "2", Password = "2" }
   
    
    
     
                    );
        }
        


    }
}
