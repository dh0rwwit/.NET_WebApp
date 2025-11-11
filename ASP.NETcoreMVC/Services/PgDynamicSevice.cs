using ASP.NETcoreMVC.Models.Dynamic;
using ASP.NETcoreMVC.Services.Interface;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace ASP.NETcoreMVC.Services
{
    public class PgDynamicService : IPgDynamicService
    {

        private readonly NpgsqlDataSource _datasource;

        public PgDynamicService(NpgsqlDataSource datasource)
        {
            _datasource = datasource;
        }


        public async Task<Dictionary<string, List<DbParamType>>> ExecuteIPgDynamic(
            string strFunctionName,
            List<DbParamType> prms
        )
        {
            // 함수명, 함수에 들어갈 매개변수
            var result = new Dictionary<string, List<DbParamType>>();

            // ?
            await using var conn = await _datasource.OpenConnectionAsync(); // NpgsqlConnection(_connStr);
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;

            await conn.OpenAsync();
            // 파라미터 자리표시자 & 추가
            var placeholders = new List<string>();
            foreach (var (p, idx) in prms.Select((p, i) => (p, i)))
            {
                var name = $"@p{idx}";
                placeholders.Add(name);

                var np = new NpgsqlParameter(name, p.Value ?? DBNull.Value)
                {
                    Direction = ParameterDirection.Input // PG에선 보통 INPUT
                };
                if (p.Dbtype.HasValue)
                    np.NpgsqlDbType = p.Dbtype.Value;   // ← NpgsqlDbType 지정

                cmd.Parameters.Add(np);
            }

            var isCall = strFunctionName.TrimStart().StartsWith("call ", StringComparison.OrdinalIgnoreCase);

            if (isCall)
            {
                // 프로시저 호출
                var procName = strFunctionName.Trim();
                // "call schema.proc" 형태로 들어왔다고 가정, 뒤에 괄호와 파라미터 붙임
                if (!procName.Contains("("))
                    procName = $"{procName}({string.Join(", ", placeholders)})";

                cmd.CommandText = procName;

                // 프로시저는 보통 반환값이 없으므로 NonQuery
                // (만약 결과셋을 반환한다면 아래처럼 리더로 읽도록 바꾸면 됨)
                await cmd.ExecuteNonQueryAsync();
                return result; // 빈 딕셔너리
            }
            else
            {
                // 함수 호출: OUT 파라미터/리턴값은 결과셋으로 반환
                // prms에 Output이 있는지에 따라 select * vs select 단일값
                var hasOutput = prms.Any(p =>
                    p.Direction == ParameterDirection.Output ||
                    p.Direction == ParameterDirection.InputOutput);

                var funcSql = hasOutput
                    ? $"select * from {strFunctionName}({string.Join(", ", placeholders)})"
                    : $"select {strFunctionName}({string.Join(", ", placeholders)})";

                cmd.CommandText = funcSql;

                await using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return result; // 결과 없음

                // 첫 행 기준으로 컬럼 -> DbParamType 매핑
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var colName = reader.GetName(i);
                    var val = reader.GetValue(i);

                    var item = new DbParamType
                    {
                        Name = colName,
                        Value = val is DBNull ? null : val,
                        Direction = ParameterDirection.Output,
                        Dbtype = null // 필요시 매핑
                    };

                    result[colName] = new List<DbParamType> { item };
                }

                return result;
            }
        }

    }
}
