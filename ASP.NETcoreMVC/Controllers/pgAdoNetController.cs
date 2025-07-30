using ASP.NETcoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using ASP.NETcoreMVC.Models;
using Npgsql;
using System.Threading.Tasks;
namespace ASP.NETcoreMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class pgAdoNetController : ControllerBase
    {
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
    
    }


}
