using ASP.NETcoreMVC.Services;

Environment.SetEnvironmentVariable("ASPNETCORE_URLS", null);


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// CORS 허용 설정
builder.Services.AddCors(options =>
{
    options.AddPolicy
    (
        "AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.WebHost.ConfigureKestrel(options =>
{
    /*
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTPS 포트
        });

        options.ListenAnyIP(5000); // HTTP 포트
    */

    options.ListenLocalhost
    (
        5001, listenOpion =>
        {
            listenOpion.UseHttps();
        }
    );
});

builder.Services.AddScoped<pgAdoNetService>();

// 앱 생성
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing결정
app.UseRouting();

// CORS 사용 - UseRouting 위에 있으면 안됨.
// UseRouting에서 어떤 경로로 요청이 들어왔는지 파악하고, 이 정보가 있어야 Cors정책이 적용될 수 있다.
app.UseCors("AllowReactApp");
// Cors미들웨어는 라우팅 이후, 엔드포인트 실행 전에

// app.UseAuthentication();
app.UseAuthorization(); // 인증/인허가


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
