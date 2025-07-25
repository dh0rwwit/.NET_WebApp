using Microsoft.AspNetCore.Mvc;

namespace ASP.NETcoreMVC.Controllers
{
    [Route("api/test")]
    public class MVC_Empty_Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            //return Ok(new { message = "Hello from ASP.NET Core!" });
            return Content("Hello from ASP.NET Core!", "text/plain; charset=utf-8");
        }

        [HttpPost("echo")]
        public IActionResult Echo([FromBody] object data)
        {
            return Ok(new { received = data });
        }


    }
}
