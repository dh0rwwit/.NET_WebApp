using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
//using Npgsql;
//using OnionASP.Options;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

// 1) Options 바인딩
//builder.Services.Configure<AppOptions>(config.GetSection("App"));
//builder.Services.Configure<PgDynamicOptions>(config.GetSection("PgDynamic"));
//builder.Services.Configure<CorsOptions>(config.GetSection("Cors"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=ServerState}/{id?}")
    .WithStaticAssets();

app.UseStaticFiles();

// 루트 핑, 개발/운영 확인용
app.MapGet("/", () => Results.Ok(new
{
    env = app.Environment.EnvironmentName,
    ok = true,
    msg = "API is running"
}));
app.MapControllers(); // (기존 컨트롤러도 같이 사용 가능)

app.Run();
