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

var app = builder.Build();


// CORS 사용
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
