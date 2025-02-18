var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", (
    int? a, int? b, HttpContext ctx) => 
    $"This is YarpServer_1 {(a ?? 0) * (b ?? 0)}, " +
    $"{(ctx.Request.Headers.TryGetValue("test", out var v)?v:"")}"
);

app.UseHttpsRedirection();

app.Run();
