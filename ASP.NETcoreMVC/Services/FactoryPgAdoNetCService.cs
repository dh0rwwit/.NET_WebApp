using ASP.NETcoreMVC.Models;
using ASP.NETcoreMVC.Services.Interface;
using Npgsql;

namespace ASP.NETcoreMVC.Services
{
    public class FactoryPgCdoNetCService : IpgAdoNetService
    {
        #region Pgsql connectionstring 이용
        private readonly NpgsqlDataSource _datasource;

        public FactoryPgCdoNetCService(NpgsqlDataSource datasource)
        {
            _datasource = datasource ?? throw new InvalidOperationException("PostgresDbA 연결 문자열이 없습니다.");
        }

        public async Task<IEnumerable<string>> GetNamesAsync()
        {
            await using var conn = await _datasource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("select name from some_table", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<string>();
            while (await reader.ReadAsync())
                list.Add(reader.GetString(0));

            return list;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = new List<User>();
            return users;
        }


        #endregion
    }


}
