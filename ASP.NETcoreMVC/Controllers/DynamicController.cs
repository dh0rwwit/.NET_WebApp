using ASP.NETcoreMVC.Models.Dynamic;
using ASP.NETcoreMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETcoreMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DynamicController : ControllerBase 
    {
        private readonly IDynamicService _DynamicService;

        public DynamicController(IDynamicService funcDynamicService)
        {
            _DynamicService = funcDynamicService;
        }

        [HttpPost("requestexec")]
        public async Task<IActionResult> ExecFunc([FromBody] DbFunctionRequest request) // ? FromBody
        {
            // 인터페이스 IDynamicService의 인스턴스를 생성하여, DynamicService의 메서드 ExecutePgDynamic 불러온다.
            var dtR = await _DynamicService.ExecutePgDynamic(request.FunctionName, request.ReqPrms);
            return Ok(dtR);
                
        }
    }
}
