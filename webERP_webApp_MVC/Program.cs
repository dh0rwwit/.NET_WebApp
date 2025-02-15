
#region ReversProxy설정을 직접

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;
using System.Text;
using System.Security.Cryptography; // 추가 필요

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HttpResponse>(options =>
{
    options.Headers["Content-Type"] = "text/html; charset=utf-8";
});


// YARP 설정 추가 : Kestrel에 리버스 프록시, 로드밸런싱 기능 부여, nginx와 달리 .config가 아닌 코드 작성
//builder.Services.AddReverseProxy()
//    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddReverseProxy().LoadFromMemory(
    new[]
    {
        // client1 프록시 라우팅
        new RouteConfig
        {
            RouteId = "client1",
            ClusterId = "client1-cluster",
            Match = new RouteMatch { Path = "/client1/{**catch-all}" }
        },
        new RouteConfig // client2 프록시 라우팅
        {
            RouteId = "client2",
            ClusterId = "client2-cluster",
            Match = new RouteMatch { Path = "/client2/{**catch-all}" }
        }
    },
    new[]
    {
        new ClusterConfig // Client1 로드 밸런싱 (RoundRobin)
        {
            ClusterId = "client1-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "client1", new DestinationConfig { Address = "http://localhost:5000" } } // YARP가 바라보는 대상
            }
        },
        new ClusterConfig // Client2 단일 서버 프록시
        {
            ClusterId = "client2-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "client2", new DestinationConfig { Address = "http://localhost:5000" } }  // YARP가 바라보는 대상
            }
        }
    }
);

var app = builder.Build();

app.MapGet("/api", () => "Hello asp.net core");

app.MapReverseProxy();

// 모든 응답을 UTF-8로 강제 설정
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
    await next();
});

// 기본 루트 페이지 추가 (404 방지)
app.MapGet("/", async context =>
{
    var message = "클라이언트 프로젝트를 run하고, 서버 프로젝트의 Program.cs의 app.MapGet에 설정한 링크를 끝에 추가하세요 \n " +
    "https://localhost:5174/ -> https://localhost:5174/api \n" +
    "서버 -> 클라이언트 값 전달, 클라이언트 -> 서버로 값 전달 해보기"

    ;
    var Encode = Encoding.UTF8.GetBytes(message);
    await context.Response.Body.WriteAsync(Encode, 0, Encode.Length);
    // await context.Response.WriteAsync("클라이언트 프로젝트를 run하고, 서버 프로젝트의 Program.cs의 app.MapGet에 설정한 링크를 끝에 추가하세요");
});

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
});

app.Run();



#endregion