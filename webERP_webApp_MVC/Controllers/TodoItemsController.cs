using Microsoft.AspNetCore.Mvc;
using webERP_webApp_MVC.Models;
using System.Collections.Concurrent;


namespace webERP_webApp_MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {    
        // 예시로 간단히 메모리 컬렉션 사용
        private static readonly ConcurrentDictionary<int, TodoItem> Items = new();
        private static int NextId = 1;

        [HttpGet]
        // ActionResult<IEnumerable<TodoItem>>는 Ok(...), NotFound(), BadRequest() 같은 HTTP Response 객체로 감싸서 반환
        public ActionResult<IEnumerable<TodoItem>> GetAll() => Ok(Items.Values);//Items.Values;

        [HttpGet("{id}")]
        public ActionResult<TodoItem> Get(int id)
            => Items.TryGetValue(id, out var item) ? item : NotFound();

        [HttpPost]
        public ActionResult<TodoItem> Create(TodoItem item)
        {
            item.Id = NextId++;
            Items[item.Id] = item;
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, TodoItem item)
        {
            if (!Items.ContainsKey(id)) return NotFound();
            item.Id = id;
            Items[id] = item;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!Items.TryRemove(id, out _)) return NotFound();
            return NoContent();
        }
    }
}
