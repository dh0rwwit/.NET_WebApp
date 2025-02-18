var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", (int? a, int? b) => $"This is YarpServer_0 {(a ?? 0) + (b ?? 0)}");

app.UseHttpsRedirection();

app.Run();
