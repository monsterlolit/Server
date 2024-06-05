using Microsoft.EntityFrameworkCore;

namespace Server
{ 
    public class Bd : DbContext
    {
        public DbSet<shoes> s2 { get; set; } = null!;
            public Bd(DbContextOptions<Bd> options)
                : base(options) 
            {
                Database.EnsureCreated();
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            modelBuilder.Entity<shoes>().HasData(
                
                new shoes { Id = 1, Name = "Adi2000", Color = "Black", Brand = "Adidas", Cost = 25000, Size = 42, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo1.jpg") },
                new shoes { Id = 2, Name = "Originals Samba", Color = "Green", Brand = "Adidas", Cost = 1200, Size = 42, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo2.jpeg") },
                new shoes { Id = 3, Name = "ADIZERO SL RUNNING", Color = "White", Brand = "Adidas", Cost = 17500, Size = 42, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo3.jpg") },
                new shoes { Id = 4, Name = "AIR MAX TERRASCAPE PLUS", Color = "Black", Brand = "Nike", Cost = 16900, Size = 42, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo4.jpg") },
                new shoes { Id = 5, Name = "Air Zoom Superrep 3", Color = "White", Brand = "Nike", Cost = 31999, Size = 42, Image = File.ReadAllBytes("F:\\Projects\\Server\\pic\\photo5.jpg") }
                );
            }  
    }
}
