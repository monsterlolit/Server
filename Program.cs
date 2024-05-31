using NServiceBus.Testing;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
app.UseWelcomePage();


app.Run();
public delegate Task RequestDelegate(HttpContext context);