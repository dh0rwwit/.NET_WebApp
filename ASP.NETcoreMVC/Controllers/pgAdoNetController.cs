
using Microsoft.AspNetCore.Mvc;
using ASP.NETcoreMVC.Models;
using Npgsql;
using System.Threading.Tasks;
using ASP.NETcoreMVC.Services.Interface; // 인터페이스 추가하면서 using ASP.NETcoreMVC.Services; 를 인터페이스 네임스페이스로 수정

// 사용자가 요청한 세그먼트 받는 곳
namespace ASP.NETcoreMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class pgAdoNetController : ControllerBase
    {
        private readonly IpgAdoNetService _service;

        public pgAdoNetController(IpgAdoNetService service) 
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _service.GetNamesAsync();
            return Ok(users);
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
            var users = await _service.GetUsersAsync();
            return Ok(users);
        }

        /* 인터페이스 도입 전
                private readonly pgAdoNetService _service;

                public pgAdoNetController(pgAdoNetService service)
                {
                    _service = service;
                }

                [HttpGet]
                public async Task<IActionResult> Get()
                {
                    var users = await _service.GetNamesAsync();
                    return Ok(users);
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
                    var users = await _service.GetUsersAsync();
                    return Ok(users);
                }
        */
    }


}
