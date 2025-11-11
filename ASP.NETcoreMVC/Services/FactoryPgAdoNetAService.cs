using ASP.NETcoreMVC.Models;
using ASP.NETcoreMVC.Services.Interface;
using Npgsql;
using System.Data.Common;

namespace ASP.NETcoreMVC.Services
{
    public class FactoryPgAdoNetAService : IpgAdoNetService
    {
        private readonly string _connStr;

        public FactoryPgAdoNetAService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("PostgresDb") ?? throw new InvalidOperationException("PostgresDbA 연결 문자열이 없습니다.");
        }

        public async Task<IEnumerable<string>> GetNamesAsync()
        {
            var result = new List<string>();
            await using var conn = new NpgsqlConnection(_connStr);
            await conn.OpenAsync();

            var cmd = new NpgsqlCommand("select plantcode,plantname from masplant", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
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
                                id = reader.GetString(0),
                                name = reader.GetString(1),
                                age = reader.GetInt32(2)
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

        // List보단 IEnuerable<T>이 정보은닉에 더 좋음.
    }

}
