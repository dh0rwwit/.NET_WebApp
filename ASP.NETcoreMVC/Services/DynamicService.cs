using ASP.NETcoreMVC.Models.Dynamic;
using ASP.NETcoreMVC.Services.Interface;
using Npgsql;
using System.Data;

namespace ASP.NETcoreMVC.Services
{
    public class DynamicService : IDynamicService
    {
        private readonly string _connStr;

        public DynamicService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("PostgresDb") ?? throw new InvalidOperationException("PostgresDbA 연결 문자열이 없습니다.");
        }

        public async Task<Dictionary<string, List<DbParamType>>> ExecutePgDynamic(string strFunctionName, List<DbParamType> prms)
        {
            // 함수명, 함수에 들어갈 매개변수
            var result = new Dictionary<string, List<DbParamType>>();

            // ?
            await using var conn = new NpgsqlConnection(_connStr);
            await conn.OpenAsync();

            // 함수연결
            using var cmd = new NpgsqlCommand(strFunctionName, conn)
            {
                // ?
                CommandType = CommandType.Text,
            };

            foreach (var p in prms)
            {
                var prm = new NpgsqlParameter(p.Name, p.Value ?? DBNull.Value)
                {
                    // Direction ??
                    Direction = p.Direction
                };
                // p의 값이 pgsql의 지정된 타입이라면 값 부여
                if (p.Dbtype.HasValue)
                { prm.NpgsqlValue = p.Dbtype.Value; }

                cmd.Parameters.Add(prm);
            }
            // ExecuteNonQueryAsync 과 ExecuteNonQuery의 차이? Async의 의미와 기능적 차이?
            // await? 가 붙은 거랑 안 붙은 거 뭐가 달라지나
            await cmd.ExecuteNonQueryAsync();

            foreach (NpgsqlParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
                {
                    result[p.ParameterName] = (List<DbParamType>)p.Value;
                }
            }


            return result;
        }



        //public async Task<DataTable> ExecProcedureAysnc(string ProcName, )
        //{
        //    using var conn = new NpgsqlConnection(_connStr);
        //    using var cmd = new NpgsqlCommand("")
        //}

    }
}
