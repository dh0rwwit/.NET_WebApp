using Grpc;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Npgsql;
//using OnionASP.Options;

// 1. 최소 호스팅, 기본 객체값 불러오기 
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration; // appsettings + UserSecret + 환경변수
var env = builder.Environment; // 개발/운영


// 1.1. Options 바인딩
//builder.Services.Configure<AppOptions>(config.GetSection("App"));
//builder.Services.Configure<PgDynamicOptions>(config.GetSection("PgDynamic"));
//builder.Services.Configure<CorsOptions>(config.GetSection("Cors"));

// 2. 서비스 등록 영역(DI 컨테이너 구성),,,

// 2-1) 컨트롤러 구성 (MVC, web API)
// Controller객체, View, Model 바인드 정보들을 Program.cs DI에 등록 -> 동적CRUD만드는데 필요함.
builder.Services.AddControllersWithViews()

    .AddJsonOptions(o =>
    {
        // 필요 시 JSON 옵션 조정 (예: 소문자 camelCase 등)
        // o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
// builder.Service : DI컨테이너(Services 컬렉션)를 채우는 코드.


// 2-2) Cors설정 : appsettings의 Cors 섹션 읽기
var CorsSection = config.GetSection("Cors");
var CorsPolicyName = CorsSection
    .GetValue<string>("PolicyName") ?? "DefaultCors";
// PolicyName의 string값을 읽고, DefaultCors 키값이 없으면 Null값을 CorsPolicyName에 대입. 

var AllowedOrigins = CorsSection
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();
// ?? Array.Empty<string> : string배열 형태의 값을 반환하고 없으면 Null
var AllowedMethods = CorsSection
    .GetSection("AllowedMethods")
    .Get<string[]>() ??
    new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
// Cors에서 허용하는 API 메서드는 Get, Post, ..., Options 이게 맞나
/*
    다른 서버/도메인에서 오는  "GET", "POST", "PUT", "DELETE", "OPTIONS" 
    요청은 허용해도 된다는 의미.
*/


var AllowedHeaders = CorsSection
    .GetSection("AllowedHeaders")
    .Get<string[]>() ?? new[] { "*" };



// 주민번호, 비밀번호는 binary코드로 변환하여 저장
// 미리 약속된 코드로 DB에서 암호화 해제하여 비교후 true/false값 전달.
var AllowCredentials = CorsSection
    .GetValue<bool>("AllowCredentials");

builder.Services.AddAuthentication();

// 앱 builder에 Cors설정 값 학습시키기.
builder.Services.AddCors(options => {
    options.AddPolicy(CorsPolicyName, policy => {
        if (AllowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(AllowedOrigins)
                .WithMethods(AllowedMethods)
                .WithHeaders(AllowedHeaders);
            // ?? 앱 빌더에 appsetting에서 읽은 Cors설정 값 적용
        }
        else
        {
            // 개발환경에선 전체 Origin허용.
            policy
                .WithOrigins() 
                .WithMethods()
                .WithHeaders();
        }

        if (AllowCredentials)
        {
            policy.AllowCredentials();
        }

    });
});

// 2-3) pgsql Datasource 등록
var pgConnStr =
    Environment.GetEnvironmentVariable("CONNSTR_POSTGRES") // 먼저 launchSettings.json 읽기
    ?? config.GetConnectionString("PostgresConnection");
// 연결문자 값은 launchsettings를 먼저 읽은 후 값이 없다면 appsettings값을 읽음.
// ASP.NET core의 경우, GetConnectionString("xxx")값을 읽고 자동으로 config["ConnectionStrings:xxx"]를 읽는다.

if (string.IsNullOrWhiteSpace(pgConnStr))
{
    throw new InvalidOperationException(
        "PostgreSQL 연결 문자열이 설정되지 않았습니다. " +
        "환경변수 CONNSTR_POSTGRES 또는 appsettings의 ConnectionStrings:PostgresConnection 을 확인하세요.");
}

builder.Services.AddNpgsqlDataSource(pgConnStr);
// Npgsql.DependencyInjection Nuget추가 필요.
// thread-safe한 NpgsqlDataSource를 싱글톤으로 등록하게 됨.

// ── 캐시 + 동적 메타/실행기
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

// 2-4) 동적 메타/실행기 등록 (PgDynamicMeta, PgDynamicExecutor)
builder.Services.AddSingleton<NpgsqlDataSource>(sp =>
{
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(pgConnStr);
    // 성능/보안/타임아웃 등 추가 튜닝
    /*
        pgsql 연결 세부옵션 값을 Program.cs에서 수정할 수 있게 만든다. 
        -> dataSourceBuilder.ConnectionPoolSize = 100; // 최대 연결 수
        -> dataSourceBuilder.CommandTimeout = 30; // 기본 커맨드 타임아웃
        현재 .ConnectionPoolSize 확장메서드를 사용할 수 있는 Nuget 설치하지 않음.
        지금은 기본값으로 충분하므로 안 건드림. 
        추후 트래픽 증가하면 pool크기 조정, Timeout 연장/축소 같은 걸 여기서 수정해야 할 수 있다.
        추가로,
        SSL, TrustServerCertificate, 플러그인 / 매핑 등록 등도 여기서 함.
    */
    return dataSourceBuilder.Build();
});

// 아직 작성 안 함.
// builder.Services.AddSingleton<PgDynamicMeta>();      // 메타 캐시 용도라면 싱글톤 적합
// builder.Services.AddScoped<PgDynamicExecutor>();     // 요청 단위로 동적 실행
// 필요하다면 나중에 인터페이스도 걸 수 있음
// builder.Services.AddScoped<IPgDynamicExecutor, PgDynamicExecutor>();

// Controllers + JSON 옵션
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // 필요 시 snake_case, DateTime 형식 지정 등
        // o.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddEndpointsApiExplorer();


// 2-4) 나중에 사용 할 gRPC 등록
builder.Services.AddGrpc();
/* 아래 3가지 Nuget 필요
    Grpc.AspNetCore
    Google.Protobuf
    Grpc.Tools
*/

var app = builder.Build();


// 3. HTTP 요청 파이프라인
// 개발전용 미들웨어

if (env.IsDevelopment()) // env -> app.Environment
{
    app.UseDeveloperExceptionPage();
    //app.useSwagger();
    //app.UseSwaggerUI(opt =>
    //{
    //    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "OnionASP v1");
    //    opt.DisplayRequestDuration();
    //});
}
else
{
    // 운영 환경 기본 보안 설정들
    app.UseExceptionHandler("/error"); // 에러 핸들링용 엔드포인트를 만들었다고 가정
    app.UseHsts();
}

// 3-1) https 강제
app.UseHttpsRedirection();

// 3-3) 정적 파일 제공 (wwwroot 사용 시)
app.UseStaticFiles();

// 3-4) 라우팅
app.UseRouting();

// 3-5) CORS
app.UseCors(CorsPolicyName);
// builder에서 builder.Services.AddCors 하는거랑 app.UseCors의 차이

// builder.Services.AddCors(...) : Appsettings에서 Cors정보 값 불러오고 Cors정책 정의.
// builder라는 작업자에게 Cors정책 학습.
// app.UserCors : builder라는 작업자가 Cors정책에 맞게 app을 지음.



// 3-6) 인증/인가 (나중에 JWT 사용하게 되면)
// app.UseAuthentication();
app.UseAuthorization();

// 3-7) 엔드포인트 맵핑
// Web API / MVC 컨트롤러
app.MapControllers();

// gRPC 사용할 때 – gRPC 서비스 클래스 추가 후 활성화
// app.MapGrpcService<MyGrpcService>();
// app.MapGet("/", () => "이 서버는 gRPC 엔드포인트만 제공합니다. 클라이언트에서 호출하세요.");

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=ServerState}/{id?}")
    .WithStaticAssets();

// 루트 핑, 개발/운영 확인용
app.MapGet("/", () => Results.Ok(new
{
    env = app.Environment.EnvironmentName,
    ok = true,
    msg = "API is running"
}));


// 4) 애플리케이션 실행
app.Run();
