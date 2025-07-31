using ASP.NETcoreMVC.Models;
using ASP.NETcoreMVC.Services.Interface;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ASP.NETcoreMVC.Services
{
    public class pgAdoNetService : IpgAdoNetService
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
        public async Task<IEnumerable<string>> GetNamesAsync()
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

    // 테스트용 클래스, pgAdoNetService의 테스트용 클래스
    // UI나 서비스 호출 하는 부분에서 ASP.NETcoreMVC.Services.Interface에서 pgAdoNetService , MockpgAdoNetService 둘 다 가져다 쓸 수 있게된다.
    // 인터페이스 작성 후 Program.cs에 DI 등록.
    public class MockpgAdoNetService : IpgAdoNetService
    {
        private readonly IConfiguration _config;
        private readonly string _connStr;

        public MockpgAdoNetService(IConfiguration config)
        {
            _config = config;
            _connStr = _config.GetConnectionString("PostgresDb")
                ?? throw new InvalidOperationException("PostgresDb 연결 문자열이 설정되지 않았습니다.");
            ;
            // readonly로 전역선언된 변수, 생성자에서 한 번만 초기화 가능함.
        }
        public async Task<IEnumerable<string>> GetNamesAsync()
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


    /* 인터페이스 도입 전
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
    */
}
