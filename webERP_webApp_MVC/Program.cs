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
                { "client1", new DestinationConfig { Address = "http://localhost:5173" } } // YARP가 바라보는 대상
            }
        },
        new ClusterConfig // Client2 단일 서버 프록시
        {
            ClusterId = "client2-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "client2", new DestinationConfig { Address = "http://localhost:5173" } }  // YARP가 바라보는 대상
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
    "https://localhost:5174/ -> https://localhost:5174/api";
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



//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Yarp.ReverseProxy.Configuration;

//var builder = WebApplication.CreateBuilder(args);

//// YARP 설정 추가 : Kestrel에 리버스 프록시, 로드밸런싱 기능 부여, nginx와 달리 .config가 아닌 코드 작성
//builder.Services.AddReverseProxy().LoadFromMemory(
//    new[]
//    {
//        // client1 프록시 라우팅
//        new RouteConfig
//        {
//            RouteId = "client1",
//            ClusterId = "client1-cluster",
//            Match = new RouteMatch { Path = "/client1/{**catch-all}" }
//        },
//        new RouteConfig // client2 프록시 라우팅
//        {
//            RouteId = "client2",
//            ClusterId = "client2-cluster",
//            Match = new RouteMatch { Path = "/client2/{**catch-all}" }
//        }
//    },
//    new[]
//    {
//        new ClusterConfig // Client1 로드 밸런싱 (RoundRobin)
//        {
//            ClusterId = "client1-cluster",
//            Destinations = new Dictionary<string, DestinationConfig>
//            {
//                { "client1", new DestinationConfig { Address = "http://localhost:5173" } } // YARP가 바라보는 대상
//            }
//        },
//        new ClusterConfig // Client2 단일 서버 프록시
//        {
//            ClusterId = "client2-cluster",
//            Destinations = new Dictionary<string, DestinationConfig>
//            {
//                { "client2", new DestinationConfig { Address = "http://localhost:5173" } }  // YARP가 바라보는 대상
//            }
//        }
//    }
//);

//var app = builder.Build();

//// 기본 루트 페이지 추가 (404 방지)
//app.MapGet("/", async context =>
//{
//    await context.Response.WriteAsync("Welcome to YARP Reverse Proxy! Use /client1/ for React.");
//});

//app.UseRouting();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapReverseProxy();
//});

//app.Run();




















/* 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// YARP Reverse Proxy 설정 (React 개발 서버를 프록시)
builder.Services.AddReverseProxy().LoadFromMemory(
    new[]
    {
        // React (Vite) 프록시 라우팅
        new RouteConfig
        {
            RouteId = "react-client",
            ClusterId = "react-cluster",
            Match = new RouteMatch { Path = "/client1/{**catch-all}" }
        },
        // Angular 프록시 라우팅
        new RouteConfig
        {
            RouteId = "angular-client",
            ClusterId = "angular-cluster",
            Match = new RouteMatch { Path = "/client2/{**catch-all}" }
        }
    },
    new[]
    {
        // React (Vite) 개발 서버 프록시
        new ClusterConfig
        {
            ClusterId = "react-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "react-dev", new DestinationConfig { Address = "https://localhost:5173" } }
            }
        }
        ,
        // Angular 개발 서버 프록시
        new ClusterConfig
        {
            ClusterId = "angular-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "angular-dev", new DestinationConfig { Address = "http://localhost:4200" } }
            }
        }
    });

var app = builder.Build();

// 기본 루트 페이지 추가 (404 방지)
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Welcome to YARP Reverse Proxy! Use /client1/ for React or /client2/ for Angular.");
});

// YARP 미들웨어 적용
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy(); // YARP 적용
});

app.Run();
*/

/*
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// YARP Reverse Proxy 설정 : React(3000, 3001) 및 Angular(4200) 앱을 프록시 함.
// Kestrel이 실행될 URL과 프록시 URL이 일치하도록 launchSettings.json을 설정해야 함.
builder.Services.AddReverseProxy().LoadFromMemory(
    new[]
    {
        // Client1 (React) 프록시 라우팅
        new RouteConfig
        {
            RouteId = "client1",
            ClusterId = "client1-cluster",
            Match = new RouteMatch { Path = "/client1/{**catch-all}" }
        },
        // Client2 (Angular) 프록시 라우팅
        new RouteConfig
        {
            RouteId = "client2",
            ClusterId = "client2-cluster",
            Match = new RouteMatch { Path = "/client2/{**catch-all}" }
        }
    },
    new[]
    {
        // Client1 로드 밸런싱 (React 앱)
        new ClusterConfig
        {
            ClusterId = "client1-cluster",
            LoadBalancingPolicy = "RoundRobin",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "client1-1", new DestinationConfig { Address = "http://localhost:5000" } }, // client1→ React 앱 (http://localhost:3000, http://localhost:3001 라운드 로빈) -> http://localhost:5000/client1/ → React (3000, 3001 번갈아 요청)
                { "client1-2", new DestinationConfig { Address = "https://localhost:5001" } } // client2 → Angular 앱 (http://localhost:4200) -> http://localhost:5000/client2/ → Angular (4200)
            }
        },
        // Client2 (Angular 앱) 단일 서버 프록시
        new ClusterConfig
        {
            ClusterId = "client2-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "client2", new DestinationConfig { Address = "http://localhost:4200" } }
            }
        }
    });

var app = builder.Build();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Welcome to YARP Reverse Proxy! Use /client1/ for React or /client2/ for Angular.");
});
// 미들웨어 설정

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy(); // YARP 적용
});

app.Run();

*/




#region  초기 생성
/*

namespace webERP_webApp_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

 */
#endregion

#region 에러발생
/* 
    1. 에러발생 : System.ArgumentException: 'More than one Yarp.ReverseProxy.Health.IPassiveHealthCheckPolicy found with the same identifier. Arg_ParamName_Name' 
        - 원인 : AddReverseProxy().LoadFromMemory(...)를 중복 호출
var builder = WebApplication.CreateBuilder(args);

// YARP 설정 추가 : Kestrel에 리버스 프록시, 로드밸런싱 기능 부여, nginx와 달리 .config가 아닌 코드 작성
builder.Services.AddReverseProxy().LoadFromMemory(
    new[]
    {
        new RouteConfig
        {
            RouteId = "client1",
            ClusterId = "client1-cluster",
            Match = new RouteMatch { Path = "/client1/{**catch-all}" }
        },
        new RouteConfig
        {
            RouteId = "client2",
            ClusterId = "client2-cluster",
            Match = new RouteMatch { Path = "/client2/{**catch-all}" }
        }
    },
    new[]
    {
        new ClusterConfig
        {
            ClusterId = "client1-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "client1", new DestinationConfig { Address = "http://localhost:3000" } }
            }
        },
        new ClusterConfig
        {
            ClusterId = "client2-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "client2", new DestinationConfig { Address = "http://localhost:4200" } }
            }
        }
    }
);

// YARP 프록시 적용
builder.Services.AddReverseProxy().LoadFromMemory(
        new[]
        {
            new RouteConfig
            {
                RouteId = "client1",
                ClusterId = "client1-cluster",
                Match = new RouteMatch { Path = "/client1/{**catch-all}" }
            }
        },
        new[]
        {
            new ClusterConfig
            {
                ClusterId = "client1-cluster",
                LoadBalancingPolicy = "RoundRobin",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "client1-1", new DestinationConfig { Address = "http://localhost:3000" } },
                    { "client1-2", new DestinationConfig { Address = "http://localhost:3001" } }
                }
            }
        });



var app = builder.Build();


app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
});

app.Run();*/
#endregion



