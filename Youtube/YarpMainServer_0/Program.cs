// Program.cs는 웹앱의 Main 파일이다.
// Properties > launchSettings.json => 웹앱 실행 호스트 번호 지정
// 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    ;

var app = builder.Build();

app.MapGet("/api", () => "Set the EndPoint api!");
app.MapGet("/1/legacy", () => "Replaced!"); // appsettings.json 설정 확인.
app.MapReverseProxy();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.Run();


#region {*rest} 의 의미
/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddReverseProxy();

var app = builder.Build();

app.MapGet("/api", () => "Set the EndPoint api!");

app.MapForwarder("{*rest}", "http://127.0.0.1");
//// rest란, /로 구분된 나머지 모든 부분을 의미

예를들면, http://naver.com/path/user/resource라는 라우터 경로(URL)이 있을 때 
    { *rest}
를 사용하면 rest변수는 path/user/resource 가 된다.
    => http://127.0.0.1 뒤의 모든 세그먼트를 받아들이고 http://127.0.0.1으로 전달 할 수 있다.
     
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.Run();
*/
#endregion