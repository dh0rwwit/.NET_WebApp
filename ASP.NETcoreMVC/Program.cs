using ASP.NETcoreMVC.Services;
using ASP.NETcoreMVC.Services.Interface;
using Npgsql;
using System.Text.Json;

// for Dynamic
static Dictionary<string, object?> JsonToDict
    (JsonElement root)
{
    var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

    // JsonElement가 Object 형식인지 먼저 체크
    if (root.ValueKind != JsonValueKind.Object)
        return dict; // 빈 딕셔너리 리턴

    foreach (var p in root.EnumerateObject())
    {
        dict[p.Name] = p.Value.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number =>
                p.Value.TryGetInt64(out var l) ? l : p.Value.GetDouble(),
            JsonValueKind.Array => p.Value.EnumerateArray()
                .Select(e =>
                    e.ValueKind == JsonValueKind.String ? (object?)e.GetString() :
                    e.ValueKind == JsonValueKind.Number && e.TryGetInt64(out var li) ? li :
                    e.ValueKind == JsonValueKind.Number ? e.GetDouble() :
                    e.ValueKind == JsonValueKind.True ? true :
                    e.ValueKind == JsonValueKind.False ? false :
                    e.ToString())
                .ToArray(),
            _ => p.Value.ToString()
        };
    }

    // 모든 경로에서 return dict 보장
    return dict;
}

Environment.SetEnvironmentVariable("ASPNETCORE_URLS", null);


var builder = WebApplication.CreateBuilder(args);

// app.Developement.json에 있는 ConnectionString 값 가져오기
var connectionString = builder.Configuration
    .GetConnectionString("PostgresConnection");

// NpgsqlDataSource 등록(Connection pool 포함, 스레드 세이프)
// 2025.11.11 Nuget,Npgsql.DependencyInjection 설치, Npgsql 9.0.3 -> 9.0.4 upgrade 필요
builder.Services.AddNpgsqlDataSource(connectionString);


// MVC - Add services to the container. 
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

#region 서비스 DI 등록
// 1. 서비스, DI등록. 이렇게 단순하게 등록하면, 인터페이스당 가장 밑에 있는 구현체 하나만 등록됨.
//builder.Services.AddScoped<pgAdoNetService>();
//builder.Services.AddScoped<ASP.NETcoreMVC.Services.Interface.IpgAdoNetService, ASP.NETcoreMVC.Services.pgAdoNetService>();
//builder.Services.AddScoped<ASP.NETcoreMVC.Services.Interface.IpgAdoNetService, ASP.NETcoreMVC.Services.MockpgAdoNetService>();

// 2. appsettings.json으로부터 개발,운영 정보 받음.
//if (builder.Environment.IsDevelopment())
//{
//    builder.Services.AddScoped<IpgAdoNetService, MockpgAdoNetService>();
//}
//else
//{
//    builder.Services.AddScoped<IpgAdoNetService, pgAdoNetService>();
//}
// 3. Delegate Factory Pattern : 하나의 인터페이스에 여러개의 구현체를 가져오게 쓰는 방법. 
builder.Services.AddScoped<FactoryPgAdoNetAService>();
builder.Services.AddScoped<FactorypgAdoNetBService>(); // 미리 A,B를 등록해놔야 Func~이 제대로 작동함.
builder.Services.AddScoped<FactoryPgCdoNetCService>();
builder.Services.AddScoped<Func<string, IpgAdoNetService>>
(
    provider => key => 
    {
        return key switch
        {
            "A" => provider.GetRequiredService<FactoryPgAdoNetAService>(), // https://localhost:5001/api/factorypgadoNet/names
            "B" => provider.GetRequiredService<FactorypgAdoNetBService>(), // https://localhost:5001/api/factorypgBdoNet/names
            _ => throw new ArgumentException("Invalid key",nameof(key))
        };
    }
);
// 세그먼트를 입력하면 Controller단에서 A,B인지 값을 Program.cs로 보내고 해당 Service를 실행한다.

// Dynamic 모델 추가(DI등록) : IDynamicService의 DynamicService 메서드 등록
builder.Services.AddScoped<IDynamicService, DynamicService>();


#endregion



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
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
