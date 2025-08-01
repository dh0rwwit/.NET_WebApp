using ASP.NETcoreMVC.Models;
using ASP.NETcoreMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ASP.NETcoreMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FactorypgAdoNetController : ControllerBase
    {
        private readonly Func<string, IpgAdoNetService> _serviceFactory;
        //private readonly IConfiguration _config;
        private readonly IpgAdoNetService _service;
        //private readonly string _connStr;



        // string 자리에는 어떤 메서드를 쓸건지 결정하는 곳, UI에서 A입력하면 Program.cs에서 읽은 DI정보를 토대로 어떤 컨트롤러를 쓸건지 결정한다.
        public FactorypgAdoNetController(Func<string, IpgAdoNetService> serviceFactory)
        {
            _serviceFactory = serviceFactory;
            //_service = _serviceFactory("A"); // 메서드에서 결정하게 함

            //_connStr = _service.GetConnectionString("PostgresDb") ?? throw new InvalidOperationException("PostgresDb 연결 문자열이 설정되지 않았습니다.");
            /* IpgAdoNetService 에는 GetConnectionString() 이 없음 -> 생성자에서 꺼내쓰지 않고, 서비스 내부에서 처리하게 함.
                1. 컨트롤러는 UI에서 사용자가 어떤 세그먼트 값을 요청하는지 받는 곳, 
                2. 컨트롤러에서 DB관련 설정은 분리한다.
             */
            /*
                1. 하지만 앱이 동적으로 다른 DB 연결 문자열을 선택해야 할 경우에는 인터페이스에 GetConnectionString() 추가하는 방식이 낫다.
                - SaaS...?
             */
            // -> Controller가 DB연결을 직접 처리하는 것은 좋은 설계 원칙(책임분리)를 위반한다. 캡슐화 부분은 좀 더 알아보기..
            // DB연결은 Service가 할 일.

            // 팩토리 패턴을 적용하지 않고 만든 컨트롤러 pgAdoNetController와 코드 비교해보기.
        }
        [HttpGet("names")]
        public async Task<IActionResult> GetNames()
        {
            var names = await _service.GetNamesAsync();
            return Ok(names);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var service = _serviceFactory("A");
            var users = await service.GetUsersAsync();
            return Ok(users);
        }

        // factory pattern, /api/factorypgadonet/userft?key=A , key가 없으면 "B"가 자동으로 맵핑됨. 
        [HttpGet("userskey")]
        public async Task<IActionResult> GetUsersByKey([FromQuery] string key = "A")
        {
            var service = _serviceFactory(key);
            //var users = await _service.GetUsersAsync();
            var users = await service.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("dynamic")]
        public IActionResult GetFromA()
        {
            var svc = _serviceFactory("A"); // 또는 "B"
            return Ok($"동적으로 서비스 A 선택: {svc.GetType().Name}");
        }
    }
}
