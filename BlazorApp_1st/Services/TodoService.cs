using Client_Server_SharedLibrary.Models;
using System.Net.Http.Json;


namespace BlazorApp_1st.Services
{
    public class TodoService
    {
        private readonly HttpClient _http;

        public TodoService(HttpClient http) => _http = http;

        public async Task<IEnumerable<TodoItem>?> GetTodos()
            => await _http.GetFromJsonAsync<IEnumerable<TodoItem>>("api/todoitems");

        public async Task<TodoItem?> GetTodo(int id)
            => await _http.GetFromJsonAsync<TodoItem>($"api/todoitems/{id}");

        public async Task CreateTodo(TodoItem item)
            => await _http.PostAsJsonAsync("api/todoitems", item);

        public async Task UpdateTodo(TodoItem item)
            => await _http.PutAsJsonAsync($"api/todoitems/{item.Id}", item);

        public async Task DeleteTodo(int id)
            => await _http.DeleteAsync($"api/todoitems/{id}");
    }


}
