var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    ;
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapGet("/",
    (
        int? a,
        int? b,
        HttpContext ctx
    )
    => $"MapGet in Program.cs(링크입력예시 : https://localhost:7122/?a=10&b=5) : {(a ?? 0) * (b ?? 0)}, " +
    $"{(ctx.Request.Headers.TryGetValue("test", out var v) ? v : "")}");
app.MapGet("/api", () => "Hello vue.js");

// app.MapForwarder("{**rest}", "http://localhost:5273/");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapReverseProxy();

app.MapHealthChecks("Check");

app.Run();


/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    ;
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapGet("/", 
    (
        int? a, 
        int? b, 
        HttpContext ctx
    ) 
    => $"MapGet in Program.cs : {(a ?? 0)*(b ?? 0)}, " +
    $"{(ctx.Request.Headers.TryGetValue("test", out var v) ? v : "")}");
app.MapGet("/api", () => "Hello vue.js");

// app.MapForwarder("{**rest}", "http://localhost:5273/");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapReverseProxy();

app.MapHealthChecks("Check");

app.Run();
*/