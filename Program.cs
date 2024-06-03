using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Server;



var builder = WebApplication.CreateBuilder();
string connection = "Server=(localdb)\\mssqllocaldb;Database=BDSHOES1;Trusted_Connection=True;";
builder.Services.AddDbContext<Bd>(options => options.UseSqlServer(connection));
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/shoes", async (Bd db) => await db.s1.ToListAsync());


app.MapGet("/api/shoes/{id:int}",async (int id, Bd db) =>
{
    shoes? shoe = await db.s1.FirstOrDefaultAsync(p => p.Id == id);

    if (shoe == null)
        return Results.NotFound(new { message = "Тапок не найден" });

    return Results.Json(shoe);
});



app.Run();


