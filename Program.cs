var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) =>
{
    var response = context.Response;
    response.Headers.ContentLanguage = "ru-RU";
    response.Headers.ContentType = "text/plain; charset=utf-8";
    response.Headers.Append("secret-id", "256");    // ���������� ���������� ���������
    await response.WriteAsync("������ METANIT.COM");
});

app.Run();