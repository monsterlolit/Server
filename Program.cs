using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/accessdenied";
});

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(builder => builder.AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();



///test
///prod



app.MapGet("video/{segment}", async (HttpContext context, string segment) =>
{
    var videoPath = Path.Combine("F:\\Projects\\Server\\media\\",segment);
    if (File.Exists(videoPath))
    {
        context.Response.ContentType = "video/MP2T";
        await context.Response.SendFileAsync(videoPath);
    }
    else
    { 
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }
});

app.MapGet("/api/shoes/txt", async (context) =>
{ 
    var response = context.Response;
    await context.Response.SendFileAsync("F:\\Projects\\Server\\pic\\photo1.webp");
});

app.MapGet("/api/shoes/xlsx", async (context) =>
{
    context.Response.Headers.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    context.Response.Headers.ContentDisposition = "attachment; filename=my_xlsx.xlsx";
    await context.Response.SendFileAsync("F:\\Projects\\Server\\pic\\hello2.xlsx");
});

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

 app.MapGet("/api/shoes/image/{id:int}", async (int id,DB db) =>
 {
     Shoes? shoe = await db.Shoes.Include(s => s.Imagedb).FirstOrDefaultAsync(p => p.ImageID == id);
     if (shoe == null)
     {
         Console.WriteLine($"Shoe with ImageID {id} not found.");
         return Results.NotFound(new { message = "Без тапков" });
     }
     if (shoe.ImageID == null)
     {
         Console.WriteLine($"Image data for Shoe with ImageID {id} not found.");
         return Results.NotFound(new { message = "Изображение не найдено" });
     }
     var mimeType = GetMimeTypeFromImage(shoe.Imagedb.Image);
     if (mimeType == null)
     {
         Console.WriteLine($"Mime type for Shoe with ImageID {id} could not be determined.");
         return Results.NotFound(new { message = "Mime type не определен" });
     }
     var stream = new MemoryStream(shoe.Imagedb.Image);
     return Results.File(stream, mimeType);
 });

app.MapGet("/api/shoes/{id:int}", async (int id, DB db) =>
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

app.MapPost("/api/shoes",  async (Shoes shoe, DB db) =>
{
    await db.Shoes.AddAsync(shoe);
    await db.SaveChangesAsync();
    return shoe;
});

app.MapGet("/accessdenied", async (HttpContext context) =>
{
    context.Response.StatusCode = 403;
    await context.Response.WriteAsync("Access Denied");
});

app.MapPost("/login", async(Users dataLogin, string? returnUrl, HttpContext context, DB db) =>
{
    if (dataLogin.Login == null || dataLogin.Password == null)
        return Results.BadRequest("Что за бизнес?");

    string login = dataLogin.Login;
    string password = dataLogin.Password;

    Users? user = db.Users.FirstOrDefault(p => p.Login == login && p.Password == password);
    if (user is null) return Results.Unauthorized();
    var claims = new List<Claim>
    {
        new(ClaimsIdentity.DefaultNameClaimType, user.Login),
        new(ClaimsIdentity.DefaultRoleClaimType, user.Role)
    };
    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    await context.SignInAsync(claimsPrincipal);
    return Results.Redirect(returnUrl ?? "/");
});

app.Map("/admin", [Authorize(Roles ="Admin")] () => "hello world!");

app.Map("/", [Authorize(Roles = "Admin, User")] (HttpContext context) =>
{
    var login = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    var role = context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
    return $"Name: {login?.Value}\nRole: {role?.Value}";
});
    

app.MapGet("/logout", [Authorize(Roles = "Admin, User")] async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Данные удалены";
});

app.Run();

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
    const string KEY = "mysupersecret_secretsecretsecretkey!123";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}


