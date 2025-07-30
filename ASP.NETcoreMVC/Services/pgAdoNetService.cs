using ASP.NETcoreMVC.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ASP.NETcoreMVC.Services
{
    public class pgAdoNetService
    {
        private readonly IConfiguration _config;
        private readonly string _connStr;

        public pgAdoNetService(IConfiguration config)
        {
            _config = config;
            _connStr = _config.GetConnectionString("PostgresDb")
                ?? throw new InvalidOperationException("PostgresDb 연결 문자열이 설정되지 않았습니다.");
            ;
            // readonly로 전역선언된 변수, 생성자에서 한 번만 초기화 가능함.
        }

        public async Task<List<string>> GetNamesAsync()
        {
            var result = new List<string>();

            await using var conn = new NpgsqlConnection(_connStr);
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("SELECT plantcode, plantname FROM masplant", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(reader.GetString(0));
                result.Add(reader.GetString(1));
            }

            return result;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var userList = new List<User>();

            try
            {
                using (var conn = new NpgsqlConnection(_connStr)) // 커넥션 값을 제대로 가져오지 않으면 여기서 튕긴다.
                {
                    await conn.OpenAsync();

                    using (var cmd = new NpgsqlCommand("select id, name, age from sysuser", conn)) // 에러발생시 에러메세지 확인은...?
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            userList.Add(new User
                            {
                                id = reader.GetString(0)
                                ,name = reader.GetString(1)
                                
                                ,age = reader.GetInt32(2)
                            });
                        }
                    }
                }
                Console.WriteLine("return 전까지 도달함. 리스트 수: " + userList.Count);
                return userList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUsersAsync 예외 발생 : " + ex.Message);
                throw;
            }
        }

    }
}
