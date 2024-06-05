
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Server;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;



var builder = WebApplication.CreateBuilder();
string connection = "Server=(localdb)\\mssqllocaldb;Database=BDSHOES2;Trusted_Connection=True;";
builder.Services.AddDbContext<Bd>(options => options.UseSqlServer(connection));
builder.Services.AddCors();
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(builder => builder.AllowAnyOrigin());

app.MapGet("/api/shoes", async (Bd db, string? brand) => 
{
    if (brand == null)
    {
        return Results.Json(await db.s2.ToListAsync());
    }
    else
    {
        return Results.Json(db.s2.Where(p => p.Brand == brand));
    }
});

static string GetMimeTypeFromImage(byte[] imageData)
{
    IImageFormat format = Image.DetectFormat(imageData);
    if (format != null)
    {
        return format.DefaultMimeType;
    }
    else
    {
        return "application/octet-stream";
    }
}

app.MapGet("/api/shoes/image/{id:int}",async (int id,Bd db) =>
{
    shoes? shoe = await db.s2.FirstOrDefaultAsync(p => p.Id == id);

    var mimeType = GetMimeTypeFromImage(shoe.Image);
    if (shoe == null)
        return Results.NotFound(new { message = "Тапок не найден" });
    var stream = new MemoryStream(shoe.Image);
    return Results.File(stream, mimeType);
});



app.MapGet("/api/shoes/{id:int}",async (int id, Bd db) =>
{
    shoes? shoe = await db.s2.FirstOrDefaultAsync(p => p.Id == id);

    if (shoe == null)
        return Results.NotFound(new { message = "Тапок не найден" });

    return Results.Json(shoe);
});



app.Run();


