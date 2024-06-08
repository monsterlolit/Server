using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

var builder = WebApplication.CreateBuilder();
string connection = "Server=(localdb)\\mssqllocaldb;Database=DBSHOES;Trusted_Connection=True;";
builder.Services.AddDbContext<DB>(options => options.UseSqlServer(connection));
builder.Services.AddCors();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true,
    };
});
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(builder => builder.AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
var people = new List<Person>
{ 
    new Person("mail@mail.ru", "1"),
    new Person("mail2@mail.ru", "2")
};
///test
///prod

app.MapGet("/api/shoes/txt", async (context) =>
{ 
    var response = context.Response;
    await context.Response.SendFileAsync("F:\\Projects\\Server\\pic\\hello.txt");
});

app.MapGet("/api/shoes/xlsx", async (context) =>
{
    
    await context.Response.SendFileAsync("F:\\Projects\\Server\\pic\\hello2.xlsx");
});

app.MapPost("/login", (Person login) =>
{
    Person? person = people.FirstOrDefault(p => p.Email == login.Email && p.Password == login.Password);
    if(person is null) 
        return Results.Unauthorized();
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
    var jwt = new JwtSecurityToken(
        issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
    var response = new
    {
        acces_token = encodedJwt,
        username = person.Email
    };
    return Results.Json(response);
});
app.MapGet("/data", [Authorize] () => new { message = "Hello World!" });

app.MapGet("/api/shoes", async (DB db, string? brand) => 
{
    if (brand == null)
    {
        return Results.Json(await db.Shoes.ToListAsync());
    }
    else
    {
        return Results.Json(db.Shoes.Where(p => p.Brand == brand));
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

 app.MapGet("/api/shoes/image/{id:int}",async (int id,DB db) =>
 {
     Shoes? shoe = await db.Shoes.Include(s => s.Imagedb).FirstOrDefaultAsync(p => p.ImageID == id);
     if (shoe == null)
     {
         Console.WriteLine($"Shoe with ImageID {id} not found.");
         return Results.NotFound(new { message = "Без тапков" });
     }
     //if (shoe.ImageID == null)
     //{
     //    Console.WriteLine($"Image data for Shoe with ImageID {id} not found.");
     //    return Results.NotFound(new { message = "Изображение не найдено" });
     //}
     var mimeType = GetMimeTypeFromImage(shoe.Imagedb.Image);
     if (mimeType == null)
     {
         Console.WriteLine($"Mime type for Shoe with ImageID {id} could not be determined.");
         return Results.NotFound(new { message = "Mime type не определен" });
     }
     var stream = new MemoryStream(shoe.Imagedb.Image);
     return Results.File(stream, mimeType);
 });

app.MapGet("/api/shoes/{id:int}",async (int id, DB db) =>
{
    Shoes? shoe = await db.Shoes.FirstOrDefaultAsync(p => p.Id == id);

    if (shoe == null)
        return Results.NotFound(new { message = "Тапок не найден" });

    return Results.Json(shoe);
});

app.MapDelete("/api/shoes/{id:int}", async (int id, DB db) =>
{
    Shoes? shoe = await db.Shoes.FirstOrDefaultAsync(s => s.Id == id);
    if (shoe == null)
    {
        return Results.NotFound(new { message = "Нет тапка"});
    }
    db.Shoes.Remove(shoe);
    await db.SaveChangesAsync();
    return Results.Json(shoe);
});

app.MapPost("/api/shoes", async (Shoes shoe, DB db) =>
{
    await db.Shoes.AddAsync(shoe);
    await db.SaveChangesAsync();
    return shoe;
});

app.Run();

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
    const string KEY = "mysupersecret_secretsecretsecretkey!123";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}
record class Person(string Email, string Password);

