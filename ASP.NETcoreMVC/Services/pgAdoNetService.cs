using Npgsql;

namespace ASP.NETcoreMVC.Services
{
    public class pgAdoNetService
    {
        private readonly IConfiguration _config;

        public pgAdoNetService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<string>> GetNamesAsync()
        {
            var result = new List<string>();
            string connStr = _config.GetConnectionString("PostgresDb");

            await using var conn = new NpgsqlConnection(connStr);
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
    }
}
