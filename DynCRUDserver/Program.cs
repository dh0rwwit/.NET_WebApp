using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Npgsql;
using Grpc;
//using OnionASP.Options;

// 1. 최소 호스팅, 기본 객체값 불러오기 
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration; // appsettings + UserSecret + 환경변수
var env = builder.Environment; // 개발/운영

// 서버는 터널을 짓는 것과 같다....


// 1.1. Options 바인딩
//builder.Services.Configure<AppOptions>(config.GetSection("App"));
//builder.Services.Configure<PgDynamicOptions>(config.GetSection("PgDynamic"));
//builder.Services.Configure<CorsOptions>(config.GetSection("Cors"));

// 2. 서비스 등록 영역(DI 컨테이너 구성),,,

// 2-1) 컨트롤러 구성 (MVC, web API)
// Controller객체, View, Model 바인드 정보들을 Program.cs DI에 등록 -> 동적CRUD만드는데 필요함.
builder.Services.AddControllersWithViews()
    /*
     AddControllers() : Web API 전용, View엔진(Razor View 등...) 사용 안함 -> View() 리턴 같은거 못 씀
     AddControllersWithViews() : MVC 구조 + Web API 둘 다 리턴 가능. -> Controller + Razor View까지 셋업
     -> /Home/ServerState 같은 view 기반 페이지도 사용 할 수 있게 해준다.
     나중에 API만 쓸 거면 AddControllers()로 바꿀 수 있지만, 지금은 "상태 확인용 페이지"도 만들 생각이라면 AddControllersWithViews()가 맞음.
    */
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


// AllowedHeader가 뭔가를 허용한다는 거같고, 
// Header를 허용한다는 거 같은데 Header의 의미?
/*
    HTTP Header = request/response 에 딸려오는 메타정보
    예를들면, "AllowedHeaders": [ "Content-Type", "Authorization" ]
      - Content-Type: application/json
      - Authorization: Bearer <토큰>
      - 기타 X-Custom-Header: ...

    CORS의 "AllowedHeaders"는
    "클라이언트의 브라우저가 이 서버로 요청을 보낼 때, 어떤 헤더를 붙여도 되는지"
    를 서버가 브라우저에게 알려주는 설정이다.
*/
// ?? JWT는 뭐고, HTTP 요청/응답을 react에서 어떻게 하길래 ...??
/*
 1. JWT : Json Web Token 
    - 로그인 성공 시 서버가 만들어서 클라이언트에 넘겨주는 문자열.
    - "누구인지, 언제 발급했는지, 언제 만료되는지" 같은 정보가
    xxxxx.yyyyy.zzzzz 형태로 .을 기준으로 세 부분에 나뉘어 들어감.
 2. 로그인 흐름을 예시로 든다면, 
    - 클라이언트가 /api/auth/login 에 ID/PW 전송
    - 서버가 검증 후, JWT를 만들어서 response body에 돌려준다. 
    (
        클라이언트가 보낸 건 request body에 
        서버가 돌려주는건 response body
    )
    - 클라이언트는 그 JWT를 메모리 / localStorage / cookie 등에 저장
 3. 이 후 클라이언트가 API에 요청 시 
    Authorization: 'Bearer ${token}'
    이런 헤더를 붙여서 보낸다. 
    서버는 이 JWT를 검증해서 기존에 인증을 진행한 클라이언트임을 판단한다.
    (
        토큰이 탈취되면 해커가 해당 사용자인 척 행동 할 수 있으므로 
        Https 사용, 토큰 짧은 만료시간, refresh토큰 등 안전 저장 전략 필요...
    )
*/

/* 
예를 들어 React에서 아래와 같은 HTTP 요청 코드 작성 시,
const token = localStorage.getItem("accessToken");

const res = await fetch("https://localhost:5001/api/dynfn/get_company", {
    method: "POST",
    headers: {
        "Content-Type": "application/json",
        "Authorization": 'Bearer ${token}' // 여기서 JWT를 Authorization에 넣음
    },
    body: JSON.stringify({ companyCode: "20251111-000001" })
});
const data = await res.json();
 
브라우저가 보내는 요청에 Authorization 헤더 "Bearer"가 들어가게 되고, 
서버의 Cors정책에 
"AllowedHeaders": [ "Content-Type", "Authorization" ]
이렇게 허용해줘야 브라우저의 Cors통과가 된다.

*/

/*
?? 그러면 외부에서 내 서버에 데이터 요청 api가 오면, 내가 작성한 
"AllowedHeaders": [ "Content-Type", "Authorization" ] 
정보를 알려줘야 할까?
(
    AllowedHeaders 값을 "직접 외부에 알려줘야" 하는 것은 아니다.
    브라우저가 preflight(OPTIONS) 요청을 보내면
    서버가 응답 헤더에 Access-Control-Allow-Headers 를 포함시켜 주고,
    브라우저가 그 값을 보고 자동으로 판단한다.
    curl / Postman / 백엔드 서버에서 호출하는 경우에는
    CORS 자체를 신경 쓰지 않는다 (브라우저만 CORS를 강제한다).
)
---->
불특정 다수의 클라이언트가 내 서버에 request하면, 
내 서버가 response header에 
Access-Control-Allow-Headers: Content-Type, Authorization
를 넣어주면, 
해당 클라이언트의 브라우저들은 이걸 보고, 
내 서버에서
Content-Type, Authorization
와 같은 헤더들을 허용하는구나 라고 스스로 판단...

...그럼 다른 도메인이 내 서버에 Rest api로 요청할 때 
Content-Type, Authorization를 신경 쓸 필요가 없는게 맞는건가?

*/

// 주민번호, 비밀번호는 binary코드로 변환하여 저장
// 미리 약속된 코드로 DB에서 암호화 해제하여 비교후 true/false값 전달.
var AllowCredentials = CorsSection
    .GetValue<bool>("AllowCredentials");
/*
    if (AllowCredentials) ...
    CORS에서 "Credentials"는
      - 쿠키(cookie)
      - Authorization 헤더(토큰)
      - TLS Client 인증서
    같은 인증 관련 정보를 의미한다.

    appsettings.json의 AllowCredentials가 true이면, 
      - 클라이언트/front개발자가 본 서버와 다른 Origin(도메인/포트)에서 
        이 서버로 요청을 보낼 때,
        쿠키/Authorization 헤더 같은 인증정보 
        (예를들면 "AllowedHeaders": [ "Content-Type", "Authorization" ])
        를 포함해서 보내는 것을  허용한다는 의미.
      - 응답도 자바스크립트 코드에서 접근할 수 있게 된다.
      
      - 회원 로그인 여부 확인 등은 JWT 미들웨어 또는 쿠키 인증등의 
        인증/인가 파이프라인(UseAuthentication/UseAuthorization)이 담당한다.

    appsettings.json의 AllowCredentials=false면
      - "다른 도메인에서 오는 모든 요청을 거절"하는 것은 아니다.
      - 단지 클라이언트 브라우저가 cross-origin 요청에 credentials(쿠키/기타 보안 인증정보)를 포함하려고 하면 막음.
      - 자격 증명없이 보내는 단순 Origin/Method/Header가 CORS 정책에 맞으면 여전히 가능     
*/

/*
    주의사항으로, 
    AllowCredentials()을 쓰면 WithOrigins("*")같이 와일드카드 Origin을 사용할 수 없다.
    보안상 "모든 Origin + 인증정보 허용" 조합은 금지함.

    그래서 실제 운영에선 
    policy.WithOrigins("https://my-front.com")
          .AllowCredentials();
    처럼 정해진 프론트 도메인이랑만 인증정보를 주고받게 만든다.
 */

// Origin : scheme://host:port 조합


// 흐름을 정리해보면




#region 아래는 완전히 틀린 정리
/* 
    클라이언트가 naver.com에 접속
    1. naver.com의 도메인에 접속
    2. id, 비밀번호 입력 후 서버에 request
    3. 서버에서는 id, pw 값과 (사용자에게 보여지지 않는) AllowedHeaders값 확인. 
    pw와 같은 보안정보를 입력 란 작성 시 react화면엔 서버에서 요구하는 header값이 작성되어 있어야 함. 
    Headers값 이 작성되어 있어야 함.
        headers: {
            "Content-Type": "application/json",
            "Authorization": 'Bearer ${token}' 
        },
    4. 서버에서는 사용자가 접속한 도메인이 naver.com임을 확인하고 추가로 
    .AllowCredentials();가 붙어있는 보안정보를 주고받는 도메인인 경우 
    headers값을 확인한다.
    그리고 header값이 일치하면, 보안 정보를 response body로 전달.

    5. 
    policy.WithOrigins("https://my-front.com")
        // .AllowCredentials();
    가 붙어있지 않는 프론트 도메인인 경우 Cors에서 허용한 도메인만 일치한다면 
    headers 구성 요소가 일치하는지 확인하지 않는다. 
*/
/*
   3. 서버에서 AllowedHeaders값 확인. (X)
        - AllowedHeaders값 확인은 클라이언트 브라우저가 할 일,
        - 서버는 AllowedHeaders 목록을 브라우저에게 알려주는 것.
   3. pw와 같은 보안정보를 입력 란 작성 시 
    react화면엔 서버에서 요구하는 header값이 작성되어 있어야 함. 
    Headers값 이 작성되어 있어야 함. (X)
        - Authorization 헤더는 
        로그인 성공 후 토큰 발급받고 나서 
        클라이언트 JS가 headers에 직접 세팅하는 것
    4. 서버는 header값이 일치하면 보안 정보를 response body로 전달(X)
        - 서버는 Cors때문에 Authorization 헤더를 검사하지 않는다.
        - Authorization 헤더는 인증 미들웨어(JWT/Cookie) 가 검증하는 대상이지 
        CORS가 검증하는 대상이 아님.
    5. policy.WithOrigins(…);만 붙으면 header는 검사하지 않는다.
        - CORS는 "허용한 Origin, Method, Headers 조합"만 판단하고
        Authorization 헤더의 내용이 맞는지 틀리는지 검사하지 않는다.

*/
#region 재정리 1.
/*
불특정 다수의 사용자가 naver.com과 같은 도메인 접속, 로그인 할 때 브라우저, 서버에서의 인증 과정 정리.
- 서버 앱이 설치된 도메인
- 프론트 앱이 설치된 도메인
- 사용자(클라이언트)의 브라우저
이렇게 3가지 영역을 나누고 정리를 시작해야한다.


1. 사용자가 웹사이트(naver.com 같은 프론트 앱이 설치된 도메인)에 접속
    - 사용자 브라우저가 HTML/JS/CSS 다운로드함.
    - 여기까진 Cors와 무관 <- Cors는 API 요청 시 동작하는 것이기 때문이다.

2. 사용자가 로그인(React에서 API 호출) 버튼 클릭.
    - react예시 코드 : 
    fetch("https://api.naver.com/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ id: "...", pw: "..." })
    });

    - 이 때 react는 Authorization 헤더를 절대 자동으로 입력하지 않음.
    <- 로그인 시점엔 토큰이 없기 때문

3. 사용자 화면에서 넘어온 id,pw값을 DB에서 확인 후, 문제없으면 로그인 성공
- 서버가 JWT를 생성하여 -> response body로 클라이언트 브라우저에게 보낸다. 
- 클라이언트 브라우저는 이 JWT를 localStrage등에 저장한다. (--)
    -> 클라이언트 브라우저가 이 response body로받은 JWT를 
    react 코드가 localStorage / sessionStorage / Cookie 등에 직접 저장하게 만들어줘야 한다.
( 
    여기서 중요한 점은 
    "클라이언트 브라우저가 JWT를 절대 자동으로 저장하지 않는다는 점이다."
    - 서버가 JWT를 Response body에 담아 보내도, 
    브라우저는 그걸 자동으로 localStorage/cookie/sessionStorage에 저장하지 않는다.
    (
        response body는 "그냥 데이터"로 취급
        브라우저가 이 body를 해석하거나 자동 저장하는 것은 금지되어 있음.
        body를 읽는 것은 react js코드로 작성해줘야한다.
    )

    const res = await fetch("/auth/login", { ... });
    const data = await res.json();
    localStorage.setItem("jwt", data.token);  // ← 반드시 직접 저장해야 함
)
3.1. 브라우저가 자동 저장하는 경우는 딱 1가지 밖에 없다.
    - 서버가 Set-Cookie 헤더에 쿠키를 넣어서 보낼 때만 브라우저가 자동으로 저장한다.
    - Jwt를 response body에 담아 보내는 방식은 자동저장(X)

4. 인증이 필요한 API 호출 시
    - react가 headers에 JWT를 직접 넣음:

    const token = localStorage.getItem("jwt");

    fetch("https://api.naver.com/company/list", {
      method: "GET",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });
    - 여기서 Authorization 헤더는 react 개발자가 직접 작성해줘야 함.
    (브라우저/React가 자동으로 넣어주지 않는다)

4-1) 클라이언트 브라우저는 서버에 요청(preflight)을 보냄
    OPTIONS /company/list
    Origin: https://naver.com
    Access-Control-Request-Headers: Authorization
    Access-Control-Request-Method: GET
    - 브라우저는 Authorization 헤더의 값을 검사하지 않음.
    단지, Authorization 헤더를 붙여도 되는 서버인가?"만 묻는 것.

4-2) 서버의 Cors middleware가 사용자 브라우저에 응답.
    Access-Control-Allow-Origin: https://naver.com
    Access-Control-Allow-Headers: Content-Type, Authorization
    Access-Control-Allow-Methods: GET, POST
    Access-Control-Allow-Credentials: true
    - 이걸보고 사용자 브라우저는 
    "이 서버는 Authorization 헤더를 붙이는 걸 허용하네?
    그러면 요청을 보내도 되겠다."
    라고 결정함.

5. 실제 GET/POST 요청을 보내고 Authorization헤더의 JWT를 서버가 검증.
    - 이 부분은 CORS가 책임지는게 아니고,인증 미들웨어(JWT)의 책임.
    
    서버는 
        - CORS 미들웨어:
          Origin / Method / Headers 조합이 CORS 설정
          (AllowedOrigins / AllowedMethods / AllowedHeaders / AllowCredentials)
          에 맞는지 검사
        - 인증 미들웨어(JWT 등):
          Authorization 헤더에 담긴 JWT 토큰이 유효한지 검사
        - 권한(Authorization) 필터:
          이 사용자가 이 API를 호출할 권한이 있는지 검사

    이렇게 따로따로 처리한다.

영역	    누가 검증?          무엇을 검증?
CORS	브라우저 + 서버	    "이 헤더/메서드를 붙여도 되냐?"
인증	    서버(JWT 미들웨어)	"JWT 토큰이 유효한가?"
권한	    서버	                "이 API를 호출할 권한이 있는가?"

즉,
    CORS = "브라우저 보안 정책"
    JWT 인증 = "서버 인증 체계"
 */

#endregion

// ? 그럼 개발자가 프론트 앱을 1001 IP에 빌드하면 
// 다수의 클라이언트는 1001 IP에 접속하는게 맞아?

#endregion

/* 이 부분에대해 깊은 이해를 위해서는 
    간단한 JWT 발급/검증 미들웨어 흐름 (로그인 컨트롤러 + [Authorize] 붙은 API) 한 세트 만들어서,

    React에서 fetch/axios로 Authorization 헤더 붙여 호출하는 연습이 필요.
*/



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
            policy
                .WithOrigins() // appsettings에 AllowedOrigins를 비워놓으면 전체 Origins를 허용 : 프론트 개발자가 여러명일 경우 서버 허용 도메인은 Cors로 제한 하지 않는게 훨씬 낫다. 
                .WithMethods(AllowedMethods)
                .WithHeaders(AllowedHeaders);
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

// 
builder.Services.AddNpgsqlDataSource(pgConnStr);
// Npgsql.DependencyInjection Nuget추가 필요.

// thread-safe한 NpgsqlDataSource를 싱글톤으로 등록하게 됨.
/*
    1. 여러 HTTP요청이 동시에 같은 NpgsqlDataSource인스턴스를 사용해도 안전하다는 의미
    싱글톤으로 등록 한다는 것은, 앱 전체에서 딱 1개의 NpgsqlDataSource
    2. 서버 앱 전체에서 DB 연결 pool 인스턴스를 하나만 만들고, 
    많은 요청에서 동시에 써도 안전.
    3. 성능/메모리 측면에서 좋고 매 요청마다 NpgsqlDataSource를 생성하고 버리는 것보다 효율적.
*/


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

// 2-4) 나중에 사용 할 gRPC 등록
builder.Services.AddGrpc();
/* 아래 3가지 Nuget 필요
    Grpc.AspNetCore
    Google.Protobuf
    Grpc.Tools
*/

var app = builder.Build();
// 이러면 여기서 서버 앱 빌드가 끝나 버리는 거 아닌가?
/*
    빌더에 학습시킨 서비스, 환경설정 정보를 바탕으로, 
    앱을 짓는 단계(터널에 뼈대를 구축하는 단계)
    추가로 미들웨어 파이프라인을 계속 터널에 추가해줘야 함
    (터널에 필요한 기능)
*/
// ?? 미들웨어 파이프라인이란게 뭐야 터널에 붙이는 옵션 같은 거냐
// 터널에 붙이는 옵션 + 터널에 줄줄이 세워둔 필터

/* ASP.NET core 요청 흐름

    1. 클라이언트 -> Kestrel서버에 HTTP 요청 도착.
    2. app.UseXXX(...)로 추가한 미들웨어들이 줄줄이 요청을 처리하도록 설정.
        UseHttpsRedirection
        UseStaticFiles
        UseRouting
        UseCors
        UseAuthentication
        UseAuthorization 
        (마지막에) MapControllers / MapGet 같은 endpoint 미들웨어.
    3. 응답도 역순으로 미들웨어를 거쳐서 나간다.
    4. 코드를 보면
        app.UseHttpsRedirection(); // 1
        app.UseStaticFiles();      // 2
        app.UseRouting();          // 3
        app.UseCors(CorsPolicyName); // 4
        app.UseAuthorization();    // 5
        app.MapControllers();      // 6 (마지막 종착역)

    5. 각각의 미들웨어는 
        HTTPS로 요청안왔으면 HTTPS로 리다이렉트하고 
        /css/site.css와 같은 정적파일이면 Controller로 가지말고 wwwroot에서 바로 파일로 주고
        URL이 어떤 Controller에 맵핑해야하는지 결정하고
        Origin, Method, Header가 Cors정책에 맞는지 파악 
        이 request는 인증을 통과했는지 파악 
    이렇게 각자의 역할을 하는 필터라고 보면된다. 

    HTTP 파이프라인은 "요청이 들어와서 나갈 때 까지 거치는 미들웨어 chain"이라고 보면 된다.

    모든 클라이언트에서 오는 요청은 하나의 도메인을 통해서 간다고 봐야 하나???

*/


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

// app.UseCors(CorsPolicyName)
/*
서버에 들어오는 요청마다 Origin / Method / Header 를 검사하고 
안 맞으면 Cors에러를 내보낸다.
*/

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



// ?? var, await, const 등 설명 필요
/*
    var app = builder.Build(); -> 컴파일 시점에 오른쪽 타입을 보고 app 타입을 자동으로 결정 
    실제로는 WebApplication app = builder.Build(); 이것과 동일.

*/
/*
    await는 메서드 안에서 비동기 작업이 끝날 때 까지 기다린다는 뜻.

    await using var conn = new NpgsqlConnection(_connStr);
    await conn.OpenAsync();

    처럼 NpgsqlConnection 커넥션 정보를 가져오고 DB I/O를 기다리는 동안 스레드를 방치하지 않고,
    다른 요청에 양보한다.
    서버앱(ASP.NET core 등...)에서는 비동기 await/async 쓰는게 거의 필수수준. -> 같은 서버 자원으로 더 많은 요청을 처리 할 수 있게 함.
*/
/*
    const : 컴파일 타임 상수
    const int cnt = 100;
    값이 절대 변하지 않고, 컴파일할 때 값 100이 MaxPageSize안에 들어간다. (inline)

    반면에
    private readonly string _connStr;
    는 생성자에서 한 번만 할당.
    런타임 중에 결정될 수 있다. (환경변수에서 읽는 경우 등...)
*/

// ?? int cnt = 100; 이랑 차이점 ??
/*
    const int cnt = 100;은 반드시 선언하는 순간 값이 정해지고 이후 변경 불가.
    int cnt = 100; 그냥 지역변수, 나중에 cnt = 200; 이렇게 바꿀 수 있다.
        컴파일 때 상수로 치환되지 않고, 런타임에 스택에 잡힌 변수에 값이 저장된다.
 
    private readonly int _pagesize = 100; 환경/설정에 따라 변경 가능 한 값.
*/


// 2-3) Swagger (API 문서) – 개발환경에서만 쓸 거지만, 등록은 공통으로
// Swagger에대한 설명 후 등록하기
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(opt =>
//{
//    opt.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "OnionASP Dynamic API",
//        Version = "v1",
//        Description = "OnionASP 동적 DB 호출 기반 API"
//    });
//});


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
